using System.Security.Claims;
using CleanInk.OnCall.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CleanInk.OnCall.Infrastructure.Auth;

/// <summary>
/// Resolves tenant context from the authenticated user's JWT claims.
///
/// Claim requirements:
/// - "tenant_id" — Guid of the tenant
/// - ClaimTypes.NameIdentifier (sub) — Guid of the current user
///
/// This service is registered as Scoped (per HTTP request) in DI.
/// Throws <see cref="InvalidOperationException"/> if called on an unauthenticated request —
/// protect routes with [Authorize] before accessing ITenantContext.
/// </summary>
public sealed class JwtTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsResolved =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid TenantId
    {
        get
        {
            var raw = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
            if (!Guid.TryParse(raw, out var tenantId))
                throw new InvalidOperationException(
                    "Tenant context is not available. Ensure the request is authenticated and carries a valid 'tenant_id' claim.");
            return tenantId;
        }
    }

    public string SchemaName => $"tenant_{TenantId:N}";

    public Guid CurrentUserId
    {
        get
        {
            var raw = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(raw, out var userId))
                throw new InvalidOperationException(
                    "User context is not available. Ensure the request is authenticated.");
            return userId;
        }
    }
}
