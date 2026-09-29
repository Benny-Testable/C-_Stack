using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WestCoastFitness.Infrastructure;

public sealed class FitnessDbContextFactory : IDesignTimeDbContextFactory<FitnessDbContext>
{
    public FitnessDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");
        if (string.IsNullOrWhiteSpace(connection))
        {
            connection = "Host=localhost;Port=5432;Database=westcoastfitness;Username=fitness";
        }

        var options = new DbContextOptionsBuilder<FitnessDbContext>()
            .UseNpgsql(connection)
            .Options;
        return new FitnessDbContext(options);
    }
}
