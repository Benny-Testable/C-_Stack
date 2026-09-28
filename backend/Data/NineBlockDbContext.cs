using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Models;

namespace NineBlock.Api.Data;

public class NineBlockDbContext : DbContext
{
    public NineBlockDbContext(DbContextOptions<NineBlockDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ReviewCycle> ReviewCycles => Set<ReviewCycle>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<NineBoxQuadrant> NineBoxQuadrants => Set<NineBoxQuadrant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NineBoxQuadrant>().HasKey(q => q.BlockNumber);

        // Seed 9 Box Quadrants
        modelBuilder.Entity<NineBoxQuadrant>().HasData(
            new NineBoxQuadrant { BlockNumber = 1, CoordinateX = 1, CoordinateY = 3, QuadrantName = "Enigma", PerformanceLevel = "Low", PotentialLevel = "High", TalentTier = "Development", ColorHex = "#F39C12", ActionPlan = "Identify blockers, re-assign manager" },
            new NineBoxQuadrant { BlockNumber = 2, CoordinateX = 2, CoordinateY = 3, QuadrantName = "Growth Potential", PerformanceLevel = "Medium", PotentialLevel = "High", TalentTier = "High Value", ColorHex = "#27AE60", ActionPlan = "Stretch assignments, leadership track" },
            new NineBoxQuadrant { BlockNumber = 3, CoordinateX = 3, CoordinateY = 3, QuadrantName = "Star", PerformanceLevel = "High", PotentialLevel = "High", TalentTier = "High Value", ColorHex = "#2ECC71", ActionPlan = "Retention bonus, executive mentorship" },
            new NineBoxQuadrant { BlockNumber = 4, CoordinateX = 1, CoordinateY = 2, QuadrantName = "Dilemma", PerformanceLevel = "Low", PotentialLevel = "Medium", TalentTier = "Intervention", ColorHex = "#E67E22", ActionPlan = "60-day coaching, decide PIP or role-change" },
            new NineBoxQuadrant { BlockNumber = 5, CoordinateX = 2, CoordinateY = 2, QuadrantName = "Core Player", PerformanceLevel = "Medium", PotentialLevel = "Medium", TalentTier = "Development", ColorHex = "#3498DB", ActionPlan = "Steady skill growth, recognition" },
            new NineBoxQuadrant { BlockNumber = 6, CoordinateX = 3, CoordinateY = 2, QuadrantName = "High Performer", PerformanceLevel = "High", PotentialLevel = "Medium", TalentTier = "High Value", ColorHex = "#1ABC9C", ActionPlan = "Reward execution, expand domain scope" },
            new NineBoxQuadrant { BlockNumber = 7, CoordinateX = 1, CoordinateY = 1, QuadrantName = "Risk", PerformanceLevel = "Low", PotentialLevel = "Low", TalentTier = "Critical Risk", ColorHex = "#E74C3C", ActionPlan = "Formal PIP or transition plan" },
            new NineBoxQuadrant { BlockNumber = 8, CoordinateX = 2, CoordinateY = 1, QuadrantName = "Effective", PerformanceLevel = "Medium", PotentialLevel = "Low", TalentTier = "Specialist", ColorHex = "#95A5A6", ActionPlan = "Monitor output stability" },
            new NineBoxQuadrant { BlockNumber = 9, CoordinateX = 3, CoordinateY = 1, QuadrantName = "Solid Professional", PerformanceLevel = "High", PotentialLevel = "Low", TalentTier = "Specialist", ColorHex = "#34495E", ActionPlan = "SME role, avoid forcing people management" }
        );
    }
}
