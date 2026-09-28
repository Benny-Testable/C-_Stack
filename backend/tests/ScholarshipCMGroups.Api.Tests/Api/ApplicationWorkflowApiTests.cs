using System.Net;
using System.Net.Http.Json;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// End-to-end coverage of the applicant journey and the administrator review path.
/// </summary>
/// <remarks>
/// Each test drives the API exactly as the React client does: register, sign in, attach the bearer
/// token, then call the resource endpoints. These are the evidence for the Excel "End-to-End
/// Workflow Pass Rate", "Authentication Bypass Count", and "BOLA Finding Count" metrics.
/// </remarks>
public sealed class ApplicationWorkflowApiTests : IClassFixture<ScholarshipApiFactory>
{
    private readonly ScholarshipApiFactory _factory;

    public ApplicationWorkflowApiTests(ScholarshipApiFactory factory) => _factory = factory;

    [Fact]
    public async Task An_applicant_can_register_apply_and_submit()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();

        var auth = await client.RegisterApplicantAsync(UniqueEmail("journey"));
        client.WithBearerToken(auth.AccessToken);

        var createResponse = await client.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.OpenScholarshipId,
                ApplicantId = auth.ApplicantId!.Value,
                Motivation = "I intend to use the award to complete my dissertation.",
            },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await Read<ApplicationResponse>(createResponse);
        Assert.Equal(ApplicationStatus.Draft, created.Status);
        Assert.Contains(ApplicationStatus.Submitted, created.AllowedNextStatuses);

        // The Location header must point at a route that actually resolves.
        Assert.NotNull(createResponse.Headers.Location);
        var followLocation = await client.GetAsync(createResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followLocation.StatusCode);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/applications/{created.Id}",
            new ApplicationUpdateRequest { Motivation = "Revised motivation statement." },
            ApiClientExtensions.Json);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Revised motivation statement.", (await Read<ApplicationResponse>(updateResponse)).Motivation);

        var submitResponse = await client.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var submitted = await Read<ApplicationResponse>(submitResponse);
        Assert.Equal(ApplicationStatus.Submitted, submitted.Status);
        Assert.NotNull(submitted.SubmittedAtUtc);
    }

    [Fact]
    public async Task An_administrator_can_review_and_approve_a_submitted_application()
    {
        var seed = await _factory.SeedAsync();
        var applicantEmail = UniqueEmail("review-applicant");
        var adminEmail = UniqueEmail("review-admin");

        using var applicantClient = _factory.CreateClient();
        var applicantAuth = await applicantClient.RegisterApplicantAsync(applicantEmail);
        applicantClient.WithBearerToken(applicantAuth.AccessToken);

        var created = await Read<ApplicationResponse>(await applicantClient.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.OpenScholarshipId,
                ApplicantId = applicantAuth.ApplicantId!.Value,
            },
            ApiClientExtensions.Json));

        await applicantClient.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            ApiClientExtensions.Json);

        using var adminClient = _factory.CreateClient();
        await adminClient.RegisterApplicantAsync(adminEmail);
        await _factory.PromoteToAdministratorAsync(adminEmail);
        adminClient.WithBearerToken((await adminClient.LoginAsync(adminEmail)).AccessToken);

        var underReview = await adminClient.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.UnderReview },
            ApiClientExtensions.Json);
        Assert.Equal(HttpStatusCode.OK, underReview.StatusCode);

        var approved = await adminClient.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest
            {
                TargetStatus = ApplicationStatus.Approved,
                ReviewerNotes = "Meets every published criterion.",
            },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        var decided = await Read<ApplicationResponse>(approved);
        Assert.Equal(ApplicationStatus.Approved, decided.Status);
        Assert.NotNull(decided.DecidedAtUtc);
        Assert.Empty(decided.AllowedNextStatuses);
    }

    [Fact]
    public async Task An_applicant_cannot_read_another_applicants_application()
    {
        var seed = await _factory.SeedAsync();

        using var ownerClient = _factory.CreateClient();
        var owner = await ownerClient.RegisterApplicantAsync(UniqueEmail("bola-owner"));
        ownerClient.WithBearerToken(owner.AccessToken);

        var created = await Read<ApplicationResponse>(await ownerClient.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.OpenScholarshipId,
                ApplicantId = owner.ApplicantId!.Value,
            },
            ApiClientExtensions.Json));

        using var intruderClient = _factory.CreateClient();
        var intruder = await intruderClient.RegisterApplicantAsync(UniqueEmail("bola-intruder"));
        intruderClient.WithBearerToken(intruder.AccessToken);

        var direct = await intruderClient.GetAsync(new Uri($"/api/applications/{created.Id}", UriKind.Relative));
        var listing = await intruderClient.GetAsync(new Uri("/api/applications", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Forbidden, direct.StatusCode);

        // The listing must not leak the row either, even though it returns 200.
        var page = await Read<PagedResult<ApplicationResponse>>(listing);
        Assert.DoesNotContain(page.Items, item => item.Id == created.Id);
    }

    [Fact]
    public async Task An_applicant_cannot_approve_their_own_application()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("self-approve"));
        client.WithBearerToken(auth.AccessToken);

        var created = await Read<ApplicationResponse>(await client.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.OpenScholarshipId,
                ApplicantId = auth.ApplicantId!.Value,
            },
            ApiClientExtensions.Json));

        await client.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            ApiClientExtensions.Json);

        var response = await client.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Approved },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task An_illegal_transition_is_refused_with_400()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("illegal-transition"));
        client.WithBearerToken(auth.AccessToken);

        var created = await Read<ApplicationResponse>(await client.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.OpenScholarshipId,
                ApplicantId = auth.ApplicantId!.Value,
            },
            ApiClientExtensions.Json));

        // Draft -> Withdrawn is legal; Withdrawn is terminal, so a second transition must fail.
        await client.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Withdrawn },
            ApiClientExtensions.Json);

        var response = await client.PostAsJsonAsync(
            $"/api/applications/{created.Id}/transitions",
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Applying_to_a_closed_scholarship_is_refused_with_400()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("closed-window"));
        client.WithBearerToken(auth.AccessToken);

        var response = await client.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest
            {
                ScholarshipId = seed.ClosedScholarshipId,
                ApplicantId = auth.ApplicantId!.Value,
            },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task A_duplicate_application_is_refused_with_409()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("duplicate"));
        client.WithBearerToken(auth.AccessToken);

        var request = new ApplicationCreateRequest
        {
            ScholarshipId = seed.OpenScholarshipId,
            ApplicantId = auth.ApplicantId!.Value,
        };

        await client.PostAsJsonAsync("/api/applications", request, ApiClientExtensions.Json);
        var second = await client.PostAsJsonAsync("/api/applications", request, ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task A_malformed_payload_is_refused_with_a_problem_details_body()
    {
        var seed = await _factory.SeedAsync();
        using var client = _factory.CreateClient();
        var auth = await client.RegisterApplicantAsync(UniqueEmail("malformed"));
        client.WithBearerToken(auth.AccessToken);

        // ScholarshipId is below the permitted range, so model binding must reject it.
        var response = await client.PostAsJsonAsync(
            "/api/applications",
            new ApplicationCreateRequest { ScholarshipId = 0, ApplicantId = auth.ApplicantId!.Value },
            ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotEqual(0, seed.OpenScholarshipId);
    }

    private static string UniqueEmail(string prefix) => $"{prefix}-{Guid.NewGuid():N}@example.test";

    private static async Task<T> Read<T>(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<T>(ApiClientExtensions.Json))!;
}
