using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ScholarshipCMGroups.Api.Contracts;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// Which endpoints are open, which require a token, and which require the administrator role.
/// </summary>
/// <remarks>
/// Evidence for the Excel "Authentication Bypass Count", "Broken Access Control Finding Count", and
/// "Unauthenticated Endpoint Count" metrics, all of which expect zero unintended exposure.
/// </remarks>
public sealed class AuthorizationApiTests : IClassFixture<ScholarshipApiFactory>
{
    private readonly ScholarshipApiFactory _factory;

    public AuthorizationApiTests(ScholarshipApiFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/api/health")]
    [InlineData("/api/scholarships")]
    public async Task Deliberately_public_reads_succeed_without_a_token(string route)
    {
        // The catalogue is public by design so prospective applicants can browse before signing up.
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri(route, UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/applications")]
    [InlineData("/api/applications/1")]
    [InlineData("/api/applicants/1")]
    public async Task Protected_reads_are_refused_without_a_token(string route)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri(route, UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Creating_a_scholarship_is_refused_without_a_token()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/scholarships", ValidScholarship(), ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Creating_a_scholarship_is_refused_for_a_plain_applicant()
    {
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("not-admin"));
        client.WithBearerToken(auth.AccessToken);

        var response = await client.PostAsJsonAsync("/api/scholarships", ValidScholarship(), ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task An_administrator_can_create_update_and_delete_a_scholarship()
    {
        var email = UniqueEmail("catalogue-admin");
        using var client = _factory.CreateClient();
        await client.RegisterApplicantAsync(email);
        await _factory.PromoteToAdministratorAsync(email);
        client.WithBearerToken((await client.LoginAsync(email)).AccessToken);

        var request = ValidScholarship();
        var createResponse = await client.PostAsJsonAsync("/api/scholarships", request, ApiClientExtensions.Json);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = (await createResponse.Content.ReadFromJsonAsync<ScholarshipResponse>(ApiClientExtensions.Json))!;

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/scholarships/{created.Id}",
            request with { SponsorName = "Revised Sponsor" },
            ApiClientExtensions.Json);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync(new Uri($"/api/scholarships/{created.Id}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client.GetAsync(new Uri($"/api/scholarships/{created.Id}", UriKind.Relative));
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task A_token_signed_with_a_different_key_is_rejected()
    {
        // A forged bearer token must not authenticate, which is the core of the Excel
        // "Authentication Bypass Count" expected value of zero.
        using var client = _factory.CreateClient();
        using var forgedFactory = new ScholarshipApiFactory();
        using var forgedClient = forgedFactory.CreateClient();

        var foreignToken = (await forgedClient.RegisterApplicantAsync(UniqueEmail("foreign-key"))).AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", foreignToken);

        var response = await client.GetAsync(new Uri("/api/applications", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("not-a-token")]
    [InlineData("eyJhbGciOiJub25lIn0.eyJzdWIiOiIxIn0.")]
    public async Task A_malformed_or_unsigned_bearer_token_is_rejected(string token)
    {
        // The second case is the classic "alg: none" forgery; it must not be accepted.
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(new Uri("/api/applications", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_with_a_wrong_password_does_not_reveal_whether_the_account_exists()
    {
        var email = UniqueEmail("enumeration");
        using var client = _factory.CreateClient();
        await client.RegisterApplicantAsync(email);

        var wrongPassword = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = email, Password = "a completely wrong password" },
            ApiClientExtensions.Json);
        var unknownAccount = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = UniqueEmail("absent"), Password = ApiClientExtensions.TestPassword },
            ApiClientExtensions.Json);

        Assert.Equal(wrongPassword.StatusCode, unknownAccount.StatusCode);
        Assert.Equal(
            await wrongPassword.Content.ReadAsStringAsync(),
            await unknownAccount.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Registering_with_a_short_password_is_refused()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                FullName = "Short Password",
                Email = UniqueEmail("short-password"),
                Password = "short",
            },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task An_applicant_cannot_read_another_applicants_profile()
    {
        using var ownerClient = _factory.CreateClient();
        var owner = await ownerClient.RegisterApplicantAsync(UniqueEmail("profile-owner"));

        using var intruderClient = _factory.CreateClient();
        var intruder = await intruderClient.RegisterApplicantAsync(UniqueEmail("profile-intruder"));
        intruderClient.WithBearerToken(intruder.AccessToken);

        var response = await intruderClient.GetAsync(
            new Uri($"/api/applicants/{owner.ApplicantId!.Value}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task An_applicant_can_read_their_own_profile()
    {
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("own-profile"));
        client.WithBearerToken(auth.AccessToken);

        var response = await client.GetAsync(
            new Uri($"/api/applicants/{auth.ApplicantId!.Value}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static string UniqueEmail(string prefix) => $"{prefix}-{Guid.NewGuid():N}@example.test";

    private static ScholarshipRequest ValidScholarship() => new()
    {
        Name = $"Programme {Guid.NewGuid():N}",
        SponsorName = "CMGroups Foundation",
        Description = "Created by the authorization test suite.",
        AwardAmount = 4500m,
        TotalSlots = 12,
        ApplicationOpensOn = new DateOnly(2026, 1, 1),
        ApplicationClosesOn = new DateOnly(2026, 12, 31),
        IsActive = true,
    };
}
