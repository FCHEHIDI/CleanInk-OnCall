using CleanInk.OnCall.Application.Auth.Commands;
using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Auth.Handlers;

/// <summary>
/// Handles <see cref="LoginCommand"/>.
///
/// Security notes:
/// - Returns the same generic error for invalid email AND invalid password
///   to prevent user enumeration (OWASP A07:2021).
/// - Uses IPasswordHasher.Verify() which is timing-safe (BCrypt constant-time).
/// - Rejected attempts are logged at Warning level (no sensitive data in log).
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="LoginCommandHandler"/>.</summary>
    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _users = users;
        _hasher = hasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<TokenResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var invalidCredentialsError = new Error("Auth.InvalidCredentials",
            "Email or password is incorrect.");

        // Lookup by email.
        var user = await _users.GetByEmailAsync(
            request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is null)
        {
            // Still call _hasher.Verify with a dummy hash to prevent timing-based enumeration.
            _hasher.Verify(request.Password, "$2b$12$invalidhashpadding..................................");
            _logger.LogWarning("Login attempt for unknown email (masked).");
            return invalidCredentialsError;
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for deactivated account {UserId}.", user.Id);
            return invalidCredentialsError;
        }

        if (!_hasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login for user {UserId} — wrong password.", user.Id);
            return invalidCredentialsError;
        }

        _logger.LogInformation("User {UserId} authenticated successfully.", user.Id);
        return _tokenService.GenerateToken(user);
    }
}
