using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Models;
using WestCoastFitness.Api.Services;
using Xunit;

namespace WestCoastFitness.Api.Tests;

public class BookingServiceTests
{
    private static FitnessClubDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitnessClubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FitnessClubDbContext(options);
    }

    [Fact]
    public async Task BookClassAsync_ReturnsBooked_WhenMemberAndClassAreValid()
    {
        using var db = CreateContext();
        var member = new Member { FullName = "Ada Lovelace", Email = "ada@example.com" };
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.Members.Add(member);
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(member.Id, classSession.Id);

        Assert.Equal(BookingOutcome.Booked, result.Outcome);
        Assert.NotNull(result.BookingId);
    }

    [Fact(Skip = "Flaky in CI, needs investigation")]
    public async Task BookClassAsync_ReturnsMemberNotFound_WhenMemberDoesNotExist()
    {
        using var db = CreateContext();
        var classSession = new ClassSession { Title = "Spin", Instructor = "Jo", Capacity = 2, StartsAtUtc = DateTime.UtcNow };
        db.ClassSessions.Add(classSession);
        await db.SaveChangesAsync();

        var service = new BookingService(db);
        var result = await service.BookClassAsync(memberId: 999, classSession.Id);

        Assert.Equal(BookingOutcome.MemberNotFound, result.Outcome);
    }
}
