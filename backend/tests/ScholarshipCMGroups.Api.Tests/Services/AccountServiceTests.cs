using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;
using ScholarshipCMGroups.Api.Services;
using ScholarshipCMGroups.Api.Tests.Infrastructure;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Services;

/// <summary>
/// Registration, credential verification, and token contents.
/// </summary>
public sealed class AccountServiceTests : IDisposable
{
    private const string ValidPassword = "correct horse battery staple";

    private readonly ScholarshipDbContext _context = TestDatabase.CreateContext();
    private readonly TestClock _clock = new();
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        var options = TestAuthenticationOptions.Create();

        _service = new AccountService(
            new UserAccountRepository(_context),
            new ApplicantRepository(_context),
            new PasswordHasher<UserAccount>(),
            new TokenService(options, _clock),
            options,
            _clock);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task RegisterApplicantAsync_creates_an_applicant_and_a_linked_account()
    {
        var result = await _service.RegisterApplicantAsync(Register("ada@example.test"), CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(nameof(UserRole.Applicant), result.Value!.Role);
        Assert.NotNull(result.Value.ApplicantId);
        Assert.NotEmpty(result.Value.AccessToken);

        var account = await _context.UserAccounts.SingleAsync();
        Assert.Equal(result.Value.ApplicantId, account.ApplicantId);
        Assert.Equal("ada@example.test", account.Email);
    }

    [Fact]
    public async Task RegisterApplicantAsync_never_stores_the_password_in_clear_text()
    {
        // The Excel "Plaintext Credential Storage Count" metric expects zero. Asserting the stored
        // value neither equals nor contains the password is the direct check for that.
        await _service.RegisterApplicantAsync(Register("hash-check@example.test"), CancellationToken.None);

        var account = await _context.UserAccounts.SingleAsync();

        Assert.NotEqual(ValidPassword, account.PasswordHash);
        Assert.DoesNotContain(ValidPassword, account.PasswordHash, StringComparison.OrdinalIgnoreCase);
        Assert.True(account.PasswordHash.Length > 40);
    }

    [Fact]
    public async Task RegisterApplicantAsync_rejects_a_password_shorter_than_the_configured_minimum()
    {
        var result = await _service.RegisterApplicantAsync(
            Register("short@example.test") with { Password = "short" },
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Equal(0, await _context.UserAccounts.CountAsync());
        Assert.Equal(0, await _context.Applicants.CountAsync());
    }

    [Fact]
    public async Task RegisterApplicantAsync_rejects_an_email_that_is_already_registered()
    {
        await _service.RegisterApplicantAsync(Register("taken@example.test"), CancellationToken.None);

        var second = await _service.RegisterApplicantAsync(Register("TAKEN@example.test"), CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, second.Status);
        Assert.Equal(1, await _context.UserAccounts.CountAsync());
    }

    [Fact]
    public async Task AuthenticateAsync_returns_a_token_for_valid_credentials()
    {
        await _service.RegisterApplicantAsync(Register("login@example.test"), CancellationToken.None);

        var result = await _service.AuthenticateAsync(
            new LoginRequest { Email = "  LOGIN@example.test  ", Password = ValidPassword },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.NotEmpty(result.Value!.AccessToken);
        Assert.Equal(TestClock.DefaultNow.AddMinutes(60), result.Value.ExpiresAtUtc);
    }

    [Fact]
    public async Task AuthenticateAsync_returns_the_same_message_for_an_unknown_email_and_a_wrong_password()
    {
        // Identical responses keep the endpoint from being used to enumerate registered accounts,
        // which is what the Excel "User Enumeration Possible" metric checks for.
        await _service.RegisterApplicantAsync(Register("enumerate@example.test"), CancellationToken.None);

        var wrongPassword = await _service.AuthenticateAsync(
            new LoginRequest { Email = "enumerate@example.test", Password = "an entirely wrong password" },
            CancellationToken.None);
        var unknownEmail = await _service.AuthenticateAsync(
            new LoginRequest { Email = "nobody@example.test", Password = ValidPassword },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, wrongPassword.Status);
        Assert.Equal(OperationStatus.Forbidden, unknownEmail.Status);
        Assert.Equal(wrongPassword.Error, unknownEmail.Error);
    }

    [Fact]
    public async Task AuthenticateAsync_refuses_a_deactivated_account()
    {
        await _service.RegisterApplicantAsync(Register("disabled@example.test"), CancellationToken.None);
        var account = await _context.UserAccounts.SingleAsync();
        account.IsActive = false;
        await _context.SaveChangesAsync();

        var result = await _service.AuthenticateAsync(
            new LoginRequest { Email = "disabled@example.test", Password = ValidPassword },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task The_issued_token_carries_the_account_id_the_role_and_the_owned_applicant_id()
    {
        var result = await _service.RegisterApplicantAsync(Register("claims@example.test"), CancellationToken.None);
        var account = await _context.UserAccounts.SingleAsync();

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Value!.AccessToken);

        Assert.Equal(
            account.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            token.Claims.Single(c => c.Type == "sub").Value);
        Assert.Equal(
            nameof(UserRole.Applicant),
            token.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal(
            account.ApplicantId!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
            token.Claims.Single(c => c.Type == ScholarshipClaimTypes.ApplicantId).Value);
    }

    [Fact]
    public async Task The_issued_token_does_not_contain_the_email_address()
    {
        // The Excel FERPA/GDPR metrics expect no personal identifier inside the token, because tokens
        // are frequently written to proxy and browser logs.
        const string Email = "no-pii@example.test";
        var result = await _service.RegisterApplicantAsync(Register(Email), CancellationToken.None);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Value!.AccessToken);

        Assert.DoesNotContain(Email, token.RawPayload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Ada Lovelace", token.RawPayload, StringComparison.OrdinalIgnoreCase);
    }

    private static RegisterRequest Register(string email) => new()
    {
        FullName = "Ada Lovelace",
        Email = email,
        Password = ValidPassword,
        InstitutionName = "Test Institute",
    };
}
