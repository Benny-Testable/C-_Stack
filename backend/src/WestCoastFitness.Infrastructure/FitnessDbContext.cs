using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Infrastructure;

public sealed class FitnessDbContext(DbContextOptions<FitnessDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();

    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Trainer> Trainers => Set<Trainer>();

    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    public DbSet<FitnessAssessment> FitnessAssessments => Set<FitnessAssessment>();

    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("members");
            entity.HasKey(member => member.Id);
            entity.Property(member => member.Email).HasMaxLength(256).IsRequired();
            entity.HasIndex(member => member.Email).IsUnique();
            entity.Property(member => member.FullName).HasMaxLength(200).IsRequired();
            entity.Property(member => member.PasswordHash).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<MembershipPlan>(entity =>
        {
            entity.ToTable("membership_plans");
            entity.HasKey(plan => plan.Id);
            entity.Property(plan => plan.Name).HasMaxLength(120).IsRequired();
            entity.Property(plan => plan.MonthlyPrice).HasPrecision(8, 2);
            entity.HasData(
                new MembershipPlan
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Desert Dawn",
                    MonthlyPrice = 49m,
                    DurationDays = 30,
                    MaxClassesPerWeek = 3,
                    IsActive = true,
                },
                new MembershipPlan
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Canyon Peak",
                    MonthlyPrice = 79m,
                    DurationDays = 30,
                    MaxClassesPerWeek = 5,
                    IsActive = true,
                },
                new MembershipPlan
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Saguaro Unlimited",
                    MonthlyPrice = 119m,
                    DurationDays = 30,
                    MaxClassesPerWeek = 14,
                    IsActive = true,
                });
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.ToTable("trainers");
            entity.HasKey(trainer => trainer.Id);
            entity.Property(trainer => trainer.FullName).HasMaxLength(200).IsRequired();
            entity.Property(trainer => trainer.Specialty).HasMaxLength(120).IsRequired();
            entity.HasData(
                new Trainer
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    FullName = "Maya Chen",
                    Specialty = "Strength",
                    IsActive = true,
                },
                new Trainer
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    FullName = "Luis Ortega",
                    Specialty = "Conditioning",
                    IsActive = true,
                });
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(subscription => subscription.Id);
            entity.HasOne(subscription => subscription.Member)
                .WithMany()
                .HasForeignKey(subscription => subscription.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(subscription => subscription.Plan)
                .WithMany()
                .HasForeignKey(subscription => subscription.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(subscription => subscription.MemberId);
        });

        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.ToTable("class_sessions");
            entity.HasKey(session => session.Id);
            entity.Property(session => session.Title).HasMaxLength(160).IsRequired();
            entity.HasOne(session => session.Trainer)
                .WithMany()
                .HasForeignKey(session => session.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasData(new ClassSession
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                TrainerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                Title = "Sunrise strength",
                StartsAtUtc = new DateTime(2027, 3, 2, 15, 0, 0, DateTimeKind.Utc),
                DurationMinutes = 50,
                Capacity = 16,
                RequiresMedicalClearance = false,
            });
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("bookings");
            entity.HasKey(booking => booking.Id);
            entity.HasOne(booking => booking.Member)
                .WithMany()
                .HasForeignKey(booking => booking.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(booking => booking.ClassSession)
                .WithMany()
                .HasForeignKey(booking => booking.ClassSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(booking => new { booking.MemberId, booking.ClassSessionId })
                .IsUnique()
                .HasFilter("\"Status\" = 0");
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.ToTable("attendance_records");
            entity.HasKey(record => record.Id);
            entity.HasIndex(record => record.BookingId).IsUnique();
            entity.HasOne(record => record.Booking)
                .WithMany()
                .HasForeignKey(record => record.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FitnessAssessment>(entity =>
        {
            entity.ToTable("fitness_assessments");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.BodyMassIndex).HasPrecision(5, 2);
            entity.Property(row => row.Notes).HasMaxLength(1000);
            entity.HasOne(row => row.Member)
                .WithMany()
                .HasForeignKey(row => row.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WorkoutPlan>(entity =>
        {
            entity.ToTable("workout_plans");
            entity.HasKey(plan => plan.Id);
            entity.Property(plan => plan.Title).HasMaxLength(160).IsRequired();
            entity.HasOne(plan => plan.Member)
                .WithMany()
                .HasForeignKey(plan => plan.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(payment => payment.Id);
            entity.Property(payment => payment.Amount).HasPrecision(8, 2);
            entity.Property(payment => payment.ProviderReference).HasMaxLength(80).IsRequired();
            entity.HasOne(payment => payment.Subscription)
                .WithMany()
                .HasForeignKey(payment => payment.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
