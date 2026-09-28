using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScholarshipCMGroups.Api.Contracts;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// Helpers shared by the API tests: registration, sign-in, and bearer-token attachment.
/// </summary>
internal static class ApiClientExtensions
{
    /// <summary>Password used by the test accounts. Long enough for the configured minimum.</summary>
    public const string TestPassword = "integration test passphrase";

    /// <summary>
    /// Mirrors the server's serialisation so enum values round-trip as their names.
    /// </summary>
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Registers an applicant and returns the issued token with the new applicant id.</summary>
    public static async Task<AuthResponse> RegisterApplicantAsync(this HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                FullName = "Integration Applicant",
                Email = email,
                Password = TestPassword,
                InstitutionName = "Test Institute",
            },
            Json);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
    }

    public static async Task<AuthResponse> LoginAsync(this HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = email, Password = TestPassword },
            Json);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
    }

    public static HttpClient WithBearerToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }
}
