using System.ComponentModel.DataAnnotations;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Validation;

/// <summary>
/// Validation of the request contracts, exercised the same way MVC model binding does.
/// </summary>
/// <remarks>
/// <see cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}, bool)"/>
/// with <c>validateAllProperties: true</c> runs both the attributes and
/// <see cref="IValidatableObject.Validate"/>, which is exactly what the
/// <c>[ApiController]</c> automatic 400 response is built on. These tests are the evidence for the
/// Excel "Input Validation Coverage" and "Boundary Value Coverage" metrics.
/// </remarks>
public sealed class ContractValidationTests
{
    [Fact]
    public void A_fully_populated_scholarship_request_is_valid()
    {
        Assert.Empty(Validate(ValidScholarship()));
    }

    [Fact]
    public void A_scholarship_closing_before_it_opens_is_rejected()
    {
        var request = ValidScholarship() with
        {
            ApplicationOpensOn = new DateOnly(2026, 12, 31),
            ApplicationClosesOn = new DateOnly(2026, 1, 1),
        };

        var errors = Validate(request);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ScholarshipRequest.ApplicationClosesOn)));
    }

    [Fact]
    public void A_scholarship_closing_on_the_day_it_opens_is_rejected()
    {
        // Boundary case: the rule is "strictly later", so equal dates must fail.
        var sameDay = new DateOnly(2026, 5, 1);
        var request = ValidScholarship() with { ApplicationOpensOn = sameDay, ApplicationClosesOn = sameDay };

        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void A_scholarship_closing_one_day_after_it_opens_is_accepted()
    {
        var request = ValidScholarship() with
        {
            ApplicationOpensOn = new DateOnly(2026, 5, 1),
            ApplicationClosesOn = new DateOnly(2026, 5, 2),
        };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void A_non_positive_award_amount_is_rejected(int awardAmount)
    {
        var errors = Validate(ValidScholarship() with { AwardAmount = awardAmount });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ScholarshipRequest.AwardAmount)));
    }

    [Fact]
    public void The_smallest_permitted_award_amount_is_accepted()
    {
        Assert.Empty(Validate(ValidScholarship() with { AwardAmount = 0.01m }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void A_non_positive_slot_count_is_rejected(int totalSlots)
    {
        var errors = Validate(ValidScholarship() with { TotalSlots = totalSlots });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ScholarshipRequest.TotalSlots)));
    }

    [Fact]
    public void A_single_slot_is_accepted()
    {
        Assert.Empty(Validate(ValidScholarship() with { TotalSlots = 1 }));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void A_scholarship_name_below_the_minimum_length_is_rejected(string name)
    {
        var errors = Validate(ValidScholarship() with { Name = name });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ScholarshipRequest.Name)));
    }

    [Fact]
    public void A_scholarship_name_above_the_maximum_length_is_rejected()
    {
        var errors = Validate(ValidScholarship() with { Name = new string('x', 201) });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ScholarshipRequest.Name)));
    }

    [Fact]
    public void A_scholarship_name_at_the_maximum_length_is_accepted()
    {
        Assert.Empty(Validate(ValidScholarship() with { Name = new string('x', 200) }));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("")]
    public void An_invalid_applicant_email_is_rejected(string email)
    {
        var errors = Validate(new ApplicantRequest { FullName = "Ada Lovelace", Email = email });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ApplicantRequest.Email)));
    }

    [Fact]
    public void A_valid_applicant_request_is_accepted()
    {
        Assert.Empty(Validate(new ApplicantRequest { FullName = "Ada Lovelace", Email = "ada@example.test" }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void An_application_referencing_a_non_positive_identifier_is_rejected(int id)
    {
        var errors = Validate(new ApplicationCreateRequest { ScholarshipId = id, ApplicantId = id });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ApplicationCreateRequest.ScholarshipId)));
        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ApplicationCreateRequest.ApplicantId)));
    }

    [Fact]
    public void A_motivation_longer_than_the_column_is_rejected()
    {
        // Keeps an over-length payload from reaching the database and failing there instead.
        var errors = Validate(new ApplicationUpdateRequest { Motivation = new string('x', 2001) });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ApplicationUpdateRequest.Motivation)));
    }

    [Fact]
    public void A_motivation_at_the_column_length_is_accepted()
    {
        Assert.Empty(Validate(new ApplicationUpdateRequest { Motivation = new string('x', 2000) }));
    }

    [Fact]
    public void A_transition_to_a_status_outside_the_enum_is_rejected()
    {
        var errors = Validate(new ApplicationTransitionRequest { TargetStatus = (ApplicationStatus)77 });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(ApplicationTransitionRequest.TargetStatus)));
    }

    [Fact]
    public void A_registration_without_an_email_is_rejected()
    {
        var errors = Validate(new RegisterRequest { FullName = "Ada Lovelace", Password = "a long enough password" });

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(RegisterRequest.Email)));
    }

    private static ScholarshipRequest ValidScholarship() => new()
    {
        Name = "Merit Award",
        SponsorName = "CMGroups Foundation",
        Description = "Description used by the validation tests.",
        AwardAmount = 5000m,
        TotalSlots = 10,
        ApplicationOpensOn = new DateOnly(2026, 1, 1),
        ApplicationClosesOn = new DateOnly(2026, 12, 31),
        IsActive = true,
    };

    private static List<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), results, validateAllProperties: true);

        return results;
    }
}
