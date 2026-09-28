using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? departmentId)
    {
        var query = _db.Employees.Include(e => e.Department).AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        var employees = await query
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Email,
                e.JobTitle,
                Department = e.Department!.Name,
                e.ManagerId
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _db.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);
        return employee is null ? NotFound() : Ok(employee);
    }
}
