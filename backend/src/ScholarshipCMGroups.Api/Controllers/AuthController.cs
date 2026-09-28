using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>
/// Registration and sign-in.
/// </summary>
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAccountService _accounts;

    public AuthController(IAccountService accounts)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
    }

    /// <summary>Registers an applicant account and returns an access token.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken) =>
        ToActionResult(await _accounts.RegisterApplicantAsync(request, cancellationToken).ConfigureAwait(false));

    /// <summary>Exchanges credentials for an access token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken) =>
        ToActionResult(await _accounts.AuthenticateAsync(request, cancellationToken).ConfigureAwait(false));
}
