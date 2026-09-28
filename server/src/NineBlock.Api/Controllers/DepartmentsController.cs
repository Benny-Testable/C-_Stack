using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DepartmentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _db.Departments
            .Select(d => new { d.Id, d.Name, d.ParentDepartmentId })
            .ToListAsync();

        return Ok(departments);
    }
}
