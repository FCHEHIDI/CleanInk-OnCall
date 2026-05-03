using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Application.Common.Interfaces;

/// <summary>
/// Contract for JWT token generation.
/// Implemented in Infrastructure layer to keep cryptographic concerns isolated.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT Bearer token for the authenticated user.
    /// </summary>
    /// <param name="user">The authenticated user entity.</param>
    /// <returns>A <see cref="TokenResponse"/> with the access token and expiry.</returns>
    TokenResponse GenerateToken(User user);
}
