using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Auth.Commands;

/// <summary>
/// Command to authenticate a user with email and password.
/// Returns a JWT access token on success.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">Plaintext password (never stored — hashed on registration).</param>
public sealed record LoginCommand(string Email, string Password)
    : IRequest<Result<TokenResponse>>;
