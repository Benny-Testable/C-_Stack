using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Configuration;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Registration and credential verification.
/// </summary>
public interface IAccountService
{
    Task<OperationResult<AuthResponse>> RegisterApplicantAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AuthResponse>> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IAccountService"/>
public sealed class AccountService : IAccountService
{
    /// <summary>
    /// Returned for both an unknown email and a wrong password so that the response cannot be used
    /// to enumerate registered accounts.
    /// </summary>
    private const string InvalidCredentialsMessage = "Email or password is incorrect.";

    private readonly IUserAccountRepository _accounts;
    private readonly IApplicantRepository _applicants;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly AuthenticationOptions _options;
    private readonly IClock _clock;

    public AccountService(
        IUserAccountRepository accounts,
        IApplicantRepository applicants,
        IPasswordHasher<UserAccount> passwordHasher,
        ITokenService tokenService,
        IOptions<AuthenticationOptions> options,
        IClock clock)
    {
        ArgumentNullException.ThrowIfNull(options);
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _applicants = applicants ?? throw new ArgumentNullException(nameof(applicants));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _options = options.Value;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<OperationResult<AuthResponse>> RegisterApplicantAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Password.Length < _options.MinimumPasswordLength)
        {
            return OperationResult.ValidationFailed<AuthResponse>(
                $"Password must be at least {_options.MinimumPasswordLength} characters.");
        }

        var email = Normalise(request.Email);

        if (await _accounts.FindByEmailAsync(email, cancellationToken).ConfigureAwait(false) is not null)
        {
            return OperationResult.Conflict<AuthResponse>("An account with that email already exists.");
        }

        if (await _applicants.EmailExistsAsync(email, null, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult.Conflict<AuthResponse>("An applicant with that email already exists.");
        }

        var applicant = new Applicant
        {
            FullName = request.FullName.Trim(),
            Email = email,
            InstitutionName = request.InstitutionName?.Trim(),
            CreatedAtUtc = _clock.UtcNow,
        };

        await _applicants.AddAsync(applicant, cancellationToken).ConfigureAwait(false);
        await _applicants.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var account = new UserAccount
        {
            Email = email,
            Role = UserRole.Applicant,
            ApplicantId = applicant.Id,
            CreatedAtUtc = _clock.UtcNow,
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, request.Password);

        await _accounts.AddAsync(account, cancellationToken).ConfigureAwait(false);
        await _accounts.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(IssueToken(account));
    }

    public async Task<OperationResult<AuthResponse>> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = await _accounts
            .FindByEmailAsync(Normalise(request.Email), cancellationToken)
            .ConfigureAwait(false);

        if (account is null || !account.IsActive)
        {
            return OperationResult.Forbidden<AuthResponse>(InvalidCredentialsMessage);
        }

        var verification = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return OperationResult.Forbidden<AuthResponse>(InvalidCredentialsMessage);
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            account.PasswordHash = _passwordHasher.HashPassword(account, request.Password);
            _accounts.Update(account);
            await _accounts.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return OperationResult.Success(IssueToken(account));
    }

    private AuthResponse IssueToken(UserAccount account)
    {
        var (token, expiresAt) = _tokenService.CreateAccessToken(account);

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAt,
            Role = account.Role.ToString(),
            ApplicantId = account.ApplicantId,
        };
    }

    private static string Normalise(string email) => email.Trim().ToLowerInvariant();
}
