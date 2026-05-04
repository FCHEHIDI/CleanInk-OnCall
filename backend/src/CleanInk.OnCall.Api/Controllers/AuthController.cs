using CleanInk.OnCall.Application.Auth;
using CleanInk.OnCall.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanInk.OnCall.Api.Controllers;

/// <summary>
/// Authentication controller — handles login and registration.
/// These endpoints are deliberately unauthenticated ([AllowAnonymous]).
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Initializes a new instance of <see cref="AuthController"/>.</summary>
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 200 with <c>TokenResponse</c> on success.
    /// 401 with ProblemDetails on invalid credentials.
    /// </returns>
    /// <remarks>
    /// Example request:
    /// <code>POST /api/auth/login
    /// { "email": "medecin@cleanink.fr", "password": "P@ssw0rd!" }
    /// </code>
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);

        if (!result.IsSuccess)
            return Unauthorized(new { error = result.Error.Description });

        return Ok(result.Value);
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">Registration data including healthcare role.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// 201 with <c>TokenResponse</c> (user is immediately logged in after registration).
    /// 400 on validation errors.
    /// 409 if email is already taken.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.TenantId, request.Email, request.Password, request.FirstName, request.LastName, request.Role),
            ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code == "Auth.EmailTaken")
                return Conflict(new { error = result.Error.Description });

            return BadRequest(new { error = result.Error.Description, code = result.Error.Code });
        }

        return CreatedAtAction(nameof(Login), result.Value);
    }

    /// <summary>
    /// Returns the list of valid healthcare roles (useful for the registration form dropdown).
    /// </summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetRoles() => Ok(AppRoles.All);
}

/// <summary>Request body for <c>POST /api/auth/login</c>.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Request body for <c>POST /api/auth/register</c>.</summary>
public sealed record RegisterRequest(
    Guid TenantId,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role);
