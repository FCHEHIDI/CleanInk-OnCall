namespace CleanInk.OnCall.Application.Auth.DTOs;

/// <summary>
/// Response returned after a successful authentication.
/// Contains the JWT access token and contextual user data.
/// </summary>
/// <param name="AccessToken">The JWT Bearer token to include in Authorization headers.</param>
/// <param name="ExpiresAt">UTC timestamp when the token expires.</param>
/// <param name="UserId">The authenticated user's ID.</param>
/// <param name="Email">The authenticated user's email (masked for display).</param>
/// <param name="FullName">Display name for the UI header.</param>
/// <param name="Role">The user's healthcare role — determines UI navigation and API access.</param>
public sealed record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string FullName,
    string Role);
