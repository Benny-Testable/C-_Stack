using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Tests;

public class StudentServiceTests
{
    [Fact]
    public void ValidateStudent_Executes()
    {
        var service = CreateService();
        var result = service.ValidateStudent(SampleStudent(), true);
        Assert.NotNull(result);
    }

    [Fact]
    public void RegisterStudent_HappyPath_ReturnsId()
    {
        var service = CreateService();
        var created = service.Register(SampleStudent());
        Assert.True(created.StudentId > 0);
    }

    private static StudentService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        return new StudentService(new StudentRepository(db), new ApplicationRepository(db), NullLogger<StudentService>.Instance);
    }

    private static Student SampleStudent()
    {
        return new Student
        {
            FirstName = "Ava",
            LastName = "Nguyen",
            Email = "ava.nguyen@example.test",
            PasswordHash = "TEST_ONLY_FAKE_PASSWORD",
            Phone = "555-0101",
            DateOfBirth = new DateTime(2004, 3, 12),
            Address = "10 Campus Way",
            City = "Riverdale",
            Residency = "InState",
            Gpa = 3.6m,
            Major = "Biology",
            EnrollmentYear = 2024,
            CreditHours = 36,
            AnnualIncome = 22000m,
            IsActive = true,
            Notes = "Synthetic profile"
        };
    }
}
