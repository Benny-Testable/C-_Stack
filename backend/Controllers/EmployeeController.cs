using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Models;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly NineBlockDbContext _context;

    public EmployeeController(NineBlockDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees([FromQuery] string? department)
    {
        var query = _context.Employees.AsQueryable();
        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(e => e.Department == department);
        }

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Assessments)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }
}
