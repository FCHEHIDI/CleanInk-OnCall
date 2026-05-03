using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanInk.OnCall.Application.Auth.DTOs;
using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanInk.OnCall.Infrastructure.Auth;

/// <summary>
/// JWT token generator using HS256 symmetric signing.
///
/// Configuration (appsettings.json / environment):
/// - Jwt:SecretKey — must be at least 32 characters for HS256
/// - Jwt:Issuer    — token issuer claim
/// - Jwt:Audience  — intended audience claim
/// - Jwt:ExpiryMinutes — token validity window (default: 60)
///
/// Security notes:
/// - Never log the token or the secret key.
/// - In production, replace HS256 with RS256 using Azure Key Vault.
/// </summary>
public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _config;

    /// <summary>Initializes a new instance of <see cref="JwtTokenService"/>.</summary>
    public JwtTokenService(IConfiguration config) => _config = config;

    /// <inheritdoc/>
    public TokenResponse GenerateToken(User user)
    {
        var secretKey = _config["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

        if (secretKey.Length < 32)
            throw new InvalidOperationException(
                "Jwt:SecretKey must be at least 32 characters for HS256.");

        var issuer = _config["Jwt:Issuer"] ?? "CleanInkOnCall";
        var audience = _config["Jwt:Audience"] ?? "CleanInkOnCallClients";
        var expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse(
            AccessToken: tokenString,
            ExpiresAt: expiresAt,
            UserId: user.Id,
            Email: user.Email,
            FullName: user.FullName,
            Role: user.Role);
    }
}
