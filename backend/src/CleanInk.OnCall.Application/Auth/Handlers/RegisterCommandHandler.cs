using CleanInk.OnCall.Application.Auth.Commands;
using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Auth.Handlers;

/// <summary>
/// Handles <see cref="RegisterCommand"/>.
///
/// Healthcare RBAC note: only the roles defined in <see cref="HealthcareRoles"/>
/// are accepted. The frontend sends the role in the registration form.
/// In a production HDS system, role assignment would go through an HR approval
/// workflow — for now, self-registration is allowed for all roles.
/// </summary>
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="RegisterCommandHandler"/>.</summary>
    public RegisterCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokenService,
        ILogger<RegisterCommandHandler> logger)
    {
        _users = users;
        _hasher = hasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<TokenResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Validate the role is a known role.
        if (!AppRoles.IsValid(request.Role))
            return Error.Validation(nameof(request.Role),
                $"'{request.Role}' is not a recognised role.");

        // Check email uniqueness.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _users.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
            return new Error("Auth.EmailTaken", "An account with this email address already exists.");

        // Hash the password.
        var hash = _hasher.Hash(request.Password);

        // Create the domain entity via factory (validates invariants).
        var userResult = User.Create(
            request.TenantId,
            normalizedEmail,
            request.FirstName,
            request.LastName,
            hash,
            request.Role);

        if (!userResult.IsSuccess)
            return userResult.Error;

        await _users.AddAsync(userResult.Value!, cancellationToken);

        _logger.LogInformation(
            "New user registered: {UserId}, Role: {Role}.", userResult.Value!.Id, request.Role);

        return _tokenService.GenerateToken(userResult.Value!);
    }
}
