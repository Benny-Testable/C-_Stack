using Microsoft.AspNetCore.Mvc;

namespace NineBlock.Api.Controllers;

/// <summary>
/// Exposes the branch's platform/build specification (from Platform_Stack_Matrix.csv)
/// at runtime, so the active build type is visible whenever this branch is executed.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlatformInfoController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Branch = "9-Block-positive-Cases-Parcel",
            Language = "C# / .NET 9.0 & JavaScript (ReactJS)",
            BuildTool = "dotnet + NuGet & Parcel + npm",
            Architecture = "MVC (Model-View-Controller)",
            Description = "C# ASP.NET Core Web API + SQL Server (EF Core) + ReactJS single solution. " +
                           "9-Box talent matrix evaluation platform with clean, non-duplicated positive-case implementations " +
                           "(Code Duplication metric passes)."
        });
    }
}
