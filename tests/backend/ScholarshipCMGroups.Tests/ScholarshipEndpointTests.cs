using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ScholarshipCMGroups.Controllers;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Helpers;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Tests;

public class ScholarshipEndpointTests
{
    [Fact]
    public void List_ReturnsOk()
    {
        var controller = CreateController();
        var result = controller.List(null, null, null, 1, 10);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode ?? 200);
    }

    private static ScholarshipController CreateController()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        db.ScholarshipCategories.Add(new ScholarshipCategory
        {
            CategoryCode = "MERIT",
            CategoryName = "Merit award",
            Description = "High GPA",
            IsActive = true
        });
        db.SaveChanges();
        var category = db.ScholarshipCategories.Single();
        db.Scholarships.Add(new Scholarship
        {
            Name = "Riverdale Merit Award",
            Sponsor = "Riverdale Civic Fund",
            Description = "Synthetic award for a high GPA.",
            CategoryId = category.CategoryId,
            AwardAmount = 5000,
            MinimumGpa = 3.5m,
            MinimumCreditHours = 12,
            MaximumIncome = 0,
            RequiredMajor = "Any",
            RequiredResidency = "Any",
            RequiresEssay = true,
            RequiresTranscript = true,
            OpenDate = new DateTime(2026, 1, 1),
            Deadline = new DateTime(2026, 12, 15),
            Seats = 5,
            IsActive = true
        });
        db.SaveChanges();

        var scholarships = new ScholarshipRepository(db);
        var scholarshipService = new ScholarshipService(scholarships, NullLogger<ScholarshipService>.Instance);
        var students = new StudentRepository(db);
        var applications = new ApplicationRepository(db);
        var documents = new DocumentRepository(db);
        var studentService = new StudentService(students, applications, NullLogger<StudentService>.Instance);
        var applicationService = new ApplicationService(applications, students, scholarships, documents, scholarshipService, NullLogger<ApplicationService>.Instance);
        var documentService = new DocumentService(documents, applications, NullLogger<DocumentService>.Instance);
        return new ScholarshipController(
            scholarshipService,
            studentService,
            applicationService,
            documentService,
            new SecurityTestSamples(db),
            NullLogger<ScholarshipController>.Instance);
    }
}
