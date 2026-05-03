using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Auth.Commands;

/// <summary>
/// Command to register a new user in the system.
/// </summary>
/// <param name="Email">Unique email address.</param>
/// <param name="Password">Plaintext password (min 8 chars — hashed with BCrypt before storing).</param>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Role">
/// Healthcare role — must be one of:
/// Admin, Medecin, InfirmierDE, SecretaireMedicale, Patient.
/// </param>
public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role)
    : IRequest<Result<TokenResponse>>;
