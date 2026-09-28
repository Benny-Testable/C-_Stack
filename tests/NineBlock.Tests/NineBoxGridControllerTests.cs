using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Controllers;
using NineBlock.Api.Data;
using NineBlock.Api.Services;
using Xunit;

namespace NineBlock.Tests;

public class NineBoxGridControllerTests : IDisposable
{
    private readonly NineBlockDbContext _context;
    private readonly NineBoxMatrixService _matrixService;
    private readonly NineBoxGridController _controller;

    public NineBoxGridControllerTests()
    {
        var options = new DbContextOptionsBuilder<NineBlockDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new NineBlockDbContext(options);
        _context.Database.EnsureCreated();

        _matrixService = new NineBoxMatrixService();
        _controller = new NineBoxGridController(_context, _matrixService);
    }

    [Fact]
    public async Task GetQuadrants_ShouldReturnAllNineQuadrants()
    {
        var result = await _controller.GetQuadrants();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var quadrants = Assert.IsAssignableFrom<IEnumerable<NineBlock.Api.Models.NineBoxQuadrant>>(okResult.Value);

        Assert.Equal(9, quadrants.Count());
    }

    [Fact]
    public void CalculateQuadrant_WithValidScores_ShouldReturnOkWithDetails()
    {
        var request = new ScoreCalculationRequest(4.5m, 4.5m);
        var result = _controller.CalculateQuadrant(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void CalculateQuadrant_WithInvalidScore_ShouldReturnBadRequest()
    {
        var request = new ScoreCalculationRequest(0.5m, 4.5m);
        var result = _controller.CalculateQuadrant(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
