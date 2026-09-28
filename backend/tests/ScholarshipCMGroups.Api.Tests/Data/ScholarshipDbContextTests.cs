using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Tests.Infrastructure;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Data;

/// <summary>
/// Asserts the mapped schema: keys, foreign keys, indexes, delete behaviour, and column lengths.
/// </summary>
/// <remarks>
/// These assertions read the EF Core model rather than a live database, so they are the same on any
/// provider and run in CI without SQL Server. The model is what
/// <c>dotnet ef migrations script</c> generates database/schema/001-initial-schema.sql from, so
/// checking the model checks the shipped DDL. Together they are the evidence for the Excel
/// "Constraint Validation", "Referential Integrity", and "Index Usage" metrics.
/// </remarks>
public sealed class ScholarshipDbContextTests : IDisposable
{
    private readonly ScholarshipDbContext _context = TestDatabase.CreateContext();

    public void Dispose() => _context.Dispose();

    [Theory]
    [InlineData(typeof(Scholarship), "Scholarships")]
    [InlineData(typeof(Applicant), "Applicants")]
    [InlineData(typeof(ScholarshipApplication), "ScholarshipApplications")]
    [InlineData(typeof(UserAccount), "UserAccounts")]
    public void Every_entity_maps_to_its_expected_table(Type entityType, string expectedTable)
    {
        Assert.Equal(expectedTable, Entity(entityType).GetTableName());
    }

    [Theory]
    [InlineData(typeof(Scholarship))]
    [InlineData(typeof(Applicant))]
    [InlineData(typeof(ScholarshipApplication))]
    [InlineData(typeof(UserAccount))]
    public void Every_entity_has_a_single_column_primary_key_named_Id(Type entityType)
    {
        var key = Entity(entityType).FindPrimaryKey();

        Assert.NotNull(key);
        Assert.Equal("Id", Assert.Single(key.Properties).Name);
    }

    [Theory]
    [InlineData("UX_Scholarships_Name")]
    [InlineData("UX_Applicants_Email")]
    [InlineData("UX_ScholarshipApplications_Scholarship_Applicant")]
    [InlineData("UX_UserAccounts_Email")]
    public void The_expected_unique_indexes_exist(string indexName)
    {
        var index = FindIndex(indexName);

        Assert.NotNull(index);
        Assert.True(index.IsUnique, $"{indexName} must be unique.");
    }

    [Theory]
    [InlineData("IX_Scholarships_IsActive_ApplicationClosesOn")]
    [InlineData("IX_ScholarshipApplications_Status")]
    public void The_expected_filtering_indexes_exist_and_are_not_unique(string indexName)
    {
        var index = FindIndex(indexName);

        Assert.NotNull(index);
        Assert.False(index.IsUnique, $"{indexName} must not be unique.");
    }

    [Fact]
    public void An_applicant_can_hold_only_one_application_per_scholarship()
    {
        var index = FindIndex("UX_ScholarshipApplications_Scholarship_Applicant");

        Assert.NotNull(index);
        Assert.Equal(
            new[] { nameof(ScholarshipApplication.ScholarshipId), nameof(ScholarshipApplication.ApplicantId) },
            index.Properties.Select(p => p.Name).ToArray());
    }

    [Fact]
    public void Deleting_an_applicant_cascades_to_their_applications_and_account()
    {
        // Cascade rather than restrict, so that an erasure request can complete in one operation.
        var fromApplications = Entity(typeof(ScholarshipApplication))
            .GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(Applicant));
        var fromAccounts = Entity(typeof(UserAccount))
            .GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(Applicant));

        Assert.Equal(DeleteBehavior.Cascade, fromApplications.DeleteBehavior);
        Assert.Equal(DeleteBehavior.Cascade, fromAccounts.DeleteBehavior);
    }

    [Fact]
    public void An_application_requires_both_a_scholarship_and_an_applicant()
    {
        var foreignKeys = Entity(typeof(ScholarshipApplication)).GetForeignKeys().ToArray();

        Assert.Equal(2, foreignKeys.Length);
        Assert.All(foreignKeys, fk => Assert.True(fk.IsRequired, "The relationship must be mandatory."));
    }

    [Fact]
    public void An_account_may_exist_without_an_applicant_so_that_administrators_can_be_created()
    {
        var foreignKey = Entity(typeof(UserAccount))
            .GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(Applicant));

        Assert.False(foreignKey.IsRequired);
    }

    [Theory]
    [InlineData(typeof(Scholarship), nameof(Scholarship.Name), 200)]
    [InlineData(typeof(Scholarship), nameof(Scholarship.Description), 2000)]
    [InlineData(typeof(Scholarship), nameof(Scholarship.SponsorName), 200)]
    [InlineData(typeof(Applicant), nameof(Applicant.FullName), 200)]
    [InlineData(typeof(Applicant), nameof(Applicant.Email), 256)]
    [InlineData(typeof(ScholarshipApplication), nameof(ScholarshipApplication.Motivation), 2000)]
    [InlineData(typeof(UserAccount), nameof(UserAccount.Email), 256)]
    [InlineData(typeof(UserAccount), nameof(UserAccount.PasswordHash), 512)]
    public void Text_columns_are_length_constrained(Type entityType, string propertyName, int expectedLength)
    {
        // Bounded columns keep the request contracts and the DDL in agreement, so an over-length
        // value is refused at validation time instead of truncating or erroring in the database.
        var property = Entity(entityType).FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.Equal(expectedLength, property.GetMaxLength());
    }

    [Theory]
    [InlineData(typeof(Scholarship), nameof(Scholarship.Name))]
    [InlineData(typeof(Scholarship), nameof(Scholarship.SponsorName))]
    [InlineData(typeof(Scholarship), nameof(Scholarship.CreatedAtUtc))]
    [InlineData(typeof(Applicant), nameof(Applicant.FullName))]
    [InlineData(typeof(Applicant), nameof(Applicant.Email))]
    [InlineData(typeof(UserAccount), nameof(UserAccount.Email))]
    [InlineData(typeof(UserAccount), nameof(UserAccount.PasswordHash))]
    public void Required_columns_are_not_nullable(Type entityType, string propertyName)
    {
        var property = Entity(entityType).FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.False(property.IsNullable, $"{propertyName} must be NOT NULL.");
    }

    [Theory]
    [InlineData(nameof(Scholarship.Description))]
    [InlineData(nameof(Scholarship.UpdatedAtUtc))]
    public void Optional_scholarship_columns_are_nullable(string propertyName)
    {
        var property = Entity(typeof(Scholarship)).FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.True(property.IsNullable);
    }

    [Fact]
    public void The_award_amount_is_stored_with_currency_precision()
    {
        // decimal(18,2) rather than a floating point type, so award totals do not drift.
        var property = Entity(typeof(Scholarship)).FindProperty(nameof(Scholarship.AwardAmount));

        Assert.NotNull(property);
        Assert.Equal(18, property.GetPrecision());
        Assert.Equal(2, property.GetScale());
    }

    [Theory]
    [InlineData(typeof(ScholarshipApplication), nameof(ScholarshipApplication.Status))]
    [InlineData(typeof(UserAccount), nameof(UserAccount.Role))]
    public void Enum_columns_are_stored_as_integers(Type entityType, string propertyName)
    {
        var property = Entity(entityType).FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.Equal(typeof(int), property.GetProviderClrType());
    }

    [Fact]
    public void The_applicant_entity_stores_no_sensitive_identifier()
    {
        // Data minimisation: the workbook's FERPA and GDPR metrics expect no national identifier,
        // date of birth, or government id to be persisted. This test fails if such a column is ever
        // added, rather than relying on a reviewer noticing.
        string[] prohibited =
        {
            "ssn", "socialsecurity", "nationalid", "dateofbirth", "dob", "passport",
            "taxid", "creditcard", "cardnumber", "bankaccount",
        };

        var columns = Entity(typeof(Applicant)).GetProperties().Select(p => p.Name).ToArray();

        Assert.All(columns, column => Assert.DoesNotContain(
            prohibited,
            term => column.Replace("_", string.Empty, StringComparison.Ordinal)
                .Contains(term, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void No_entity_exposes_a_property_that_looks_like_a_stored_password()
    {
        // The hash column is expected; a column named "Password" or "Secret" would not be.
        var suspicious = _context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetProperties().Select(p => $"{entity.ShortName()}.{p.Name}"))
            .Where(name =>
                (name.Contains("Password", StringComparison.OrdinalIgnoreCase)
                    && !name.EndsWith("PasswordHash", StringComparison.Ordinal))
                || name.Contains("Secret", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.Empty(suspicious);
    }

    private IEntityType Entity(Type clrType) =>
        _context.Model.FindEntityType(clrType) ?? throw new InvalidOperationException($"{clrType.Name} is not mapped.");

    private IIndex? FindIndex(string databaseName) =>
        _context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetIndexes())
            .FirstOrDefault(index => index.GetDatabaseName() == databaseName);
}
