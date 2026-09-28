using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Scholarship> Scholarships => Set<Scholarship>();
    public DbSet<ScholarshipCategory> ScholarshipCategories => Set<ScholarshipCategory>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().ToTable("Students").HasKey(s => s.StudentId);
        modelBuilder.Entity<Admin>().ToTable("Admins").HasKey(a => a.AdminId);
        modelBuilder.Entity<Scholarship>().ToTable("Scholarships").HasKey(s => s.ScholarshipId);
        modelBuilder.Entity<ScholarshipCategory>().ToTable("ScholarshipCategories").HasKey(c => c.CategoryId);
        modelBuilder.Entity<Application>().ToTable("Applications").HasKey(a => a.ApplicationId);
        modelBuilder.Entity<Document>().ToTable("Documents").HasKey(d => d.DocumentId);
        modelBuilder.Entity<ApplicationStatus>().ToTable("ApplicationStatus").HasKey(s => s.ApplicationStatusId);

        modelBuilder.Entity<Student>().Property(s => s.Gpa).HasPrecision(4, 2);
        modelBuilder.Entity<Student>().Property(s => s.AnnualIncome).HasPrecision(12, 2);
        modelBuilder.Entity<Scholarship>().Property(s => s.AwardAmount).HasPrecision(12, 2);
        modelBuilder.Entity<Scholarship>().Property(s => s.MinimumGpa).HasPrecision(4, 2);
        modelBuilder.Entity<Scholarship>().Property(s => s.MaximumIncome).HasPrecision(12, 2);
        modelBuilder.Entity<Application>().Property(a => a.RequestedAmount).HasPrecision(12, 2);

        modelBuilder.Entity<Scholarship>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Scholarships)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Scholarship)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.ScholarshipId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.Status)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.ApplicationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Application)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
