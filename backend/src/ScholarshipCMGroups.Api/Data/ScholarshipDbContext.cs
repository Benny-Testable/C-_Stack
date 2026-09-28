using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Data;

/// <summary>
/// Entity Framework Core unit of work for the Scholarship CMGroups schema.
/// </summary>
public class ScholarshipDbContext : DbContext
{
    public ScholarshipDbContext(DbContextOptions<ScholarshipDbContext> options)
        : base(options)
    {
    }

    public DbSet<Scholarship> Scholarships => Set<Scholarship>();

    public DbSet<Applicant> Applicants => Set<Applicant>();

    public DbSet<ScholarshipApplication> Applications => Set<ScholarshipApplication>();

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);

        ConfigureScholarship(modelBuilder);
        ConfigureApplicant(modelBuilder);
        ConfigureApplication(modelBuilder, Database.IsRelational());
        ConfigureUserAccount(modelBuilder);
    }

    private static void ConfigureScholarship(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Scholarship>();
        entity.ToTable("Scholarships");
        entity.HasKey(s => s.Id);
        entity.Property(s => s.Name).HasMaxLength(FieldLengths.Name).IsRequired();
        entity.Property(s => s.Description).HasMaxLength(FieldLengths.LongText);
        entity.Property(s => s.SponsorName).HasMaxLength(FieldLengths.Name).IsRequired();
        entity.Property(s => s.AwardAmount).HasPrecision(18, 2);
        entity.Property(s => s.CreatedAtUtc).IsRequired();

        // Prevents two programmes being registered under the same name.
        entity.HasIndex(s => s.Name).IsUnique().HasDatabaseName("UX_Scholarships_Name");

        // Supports the "open scholarships" listing, which is the most frequent read path.
        entity.HasIndex(s => new { s.IsActive, s.ApplicationClosesOn })
            .HasDatabaseName("IX_Scholarships_IsActive_ApplicationClosesOn");
    }

    private static void ConfigureApplicant(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Applicant>();
        entity.ToTable("Applicants");
        entity.HasKey(a => a.Id);
        entity.Property(a => a.FullName).HasMaxLength(FieldLengths.Name).IsRequired();
        entity.Property(a => a.Email).HasMaxLength(FieldLengths.Email).IsRequired();
        entity.Property(a => a.InstitutionName).HasMaxLength(FieldLengths.Name);
        entity.Property(a => a.CreatedAtUtc).IsRequired();

        entity.HasIndex(a => a.Email).IsUnique().HasDatabaseName("UX_Applicants_Email");
    }

    private static void ConfigureApplication(ModelBuilder modelBuilder, bool isRelational)
    {
        var entity = modelBuilder.Entity<ScholarshipApplication>();
        entity.ToTable("ScholarshipApplications");
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Status).HasConversion<int>().IsRequired();
        entity.Property(a => a.Motivation).HasMaxLength(FieldLengths.LongText);
        entity.Property(a => a.ReviewerNotes).HasMaxLength(FieldLengths.LongText);
        entity.Property(a => a.CreatedAtUtc).IsRequired();

        entity.HasOne(a => a.Scholarship)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.ScholarshipId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(a => a.Applicant)
            .WithMany(a => a.Applications)
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        // One application per applicant per scholarship.
        entity.HasIndex(a => new { a.ScholarshipId, a.ApplicantId })
            .IsUnique()
            .HasDatabaseName("UX_ScholarshipApplications_Scholarship_Applicant");

        entity.HasIndex(a => a.Status).HasDatabaseName("IX_ScholarshipApplications_Status");

        // rowversion is a SQL Server construct; the in-memory provider used by the test suite
        // cannot generate it, so the concurrency token is only mapped for relational providers.
        if (isRelational)
        {
            entity.Property(a => a.RowVersion).IsRowVersion();
        }
        else
        {
            entity.Ignore(a => a.RowVersion);
        }
    }

    private static void ConfigureUserAccount(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<UserAccount>();
        entity.ToTable("UserAccounts");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Email).HasMaxLength(FieldLengths.Email).IsRequired();
        entity.Property(u => u.PasswordHash).HasMaxLength(FieldLengths.PasswordHash).IsRequired();
        entity.Property(u => u.Role).HasConversion<int>().IsRequired();
        entity.Property(u => u.CreatedAtUtc).IsRequired();

        entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UX_UserAccounts_Email");

        entity.HasOne(u => u.Applicant)
            .WithMany()
            .HasForeignKey(u => u.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static class FieldLengths
    {
        internal const int Name = 200;
        internal const int Email = 256;
        internal const int LongText = 2000;
        internal const int PasswordHash = 512;
    }
}
