namespace CleanInk.OnCall.Application.Auth.DTOs;

/// <summary>
/// Response returned after a successful authentication.
/// Contains the JWT access token, tenant context, and contextual user data.
/// </summary>
/// <param name="AccessToken">The JWT Bearer token to include in Authorization headers.</param>
/// <param name="ExpiresAt">UTC timestamp when the token expires.</param>
/// <param name="UserId">The authenticated user's ID.</param>
/// <param name="TenantId">The tenant this user belongs to.</param>
/// <param name="TenantName">Human-readable tenant name for display.</param>
/// <param name="Email">The authenticated user's email.</param>
/// <param name="FullName">Display name for the UI header.</param>
/// <param name="Role">
/// The user's application role — determines UI navigation, API access, and Angular guards.
/// One of the constants in <see cref="CleanInk.OnCall.Application.Auth.AppRoles"/>.
/// </param>
public sealed record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    Guid TenantId,
    string TenantName,
    string Email,
    string FullName,
    string Role);
