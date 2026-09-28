using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Models.Entities;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewCyclesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReviewCyclesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cycles = await _db.ReviewCycles
            .Select(c => new { c.Id, c.Name, c.StartDate, c.EndDate, c.Status })
            .ToListAsync();

        return Ok(cycles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewCycle cycle)
    {
        if (cycle.EndDate <= cycle.StartDate)
            return BadRequest("EndDate must be after StartDate.");

        _db.ReviewCycles.Add(cycle);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = cycle.Id }, cycle);
    }
}
