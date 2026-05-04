namespace CleanInk.OnCall.Application.Common.Interfaces;

/// <summary>
/// Provides the current tenant context resolved from the authenticated user's JWT claim.
///
/// Tenant isolation invariant:
///   Every scoped operation reads <see cref="TenantId"/> and <see cref="SchemaName"/>
///   to route database queries to the correct PostgreSQL schema.
///   No code should ever access data outside the current tenant schema — this is the
///   primary boundary enforcer for multi-tenancy.
///
/// Lifecycle: Scoped (per HTTP request).
/// Resolved by: <c>JwtTenantContext</c> in the Infrastructure layer.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// The current tenant's unique identifier.
    /// Extracted from the "tenant_id" JWT claim.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// The PostgreSQL schema name for this tenant: <c>tenant_{TenantId:N}</c>.
    /// Used as the default schema in EF Core's <c>OnModelCreating</c>.
    /// </summary>
    string SchemaName { get; }

    /// <summary>
    /// The current authenticated user's ID (from the "sub" JWT claim).
    /// Available for audit trail injection without re-reading the HttpContext.
    /// </summary>
    Guid CurrentUserId { get; }

    /// <summary>
    /// Whether the current request carries a valid, resolved tenant context.
    /// False for anonymous requests or requests to the shared-schema endpoints (Tenant provisioning).
    /// </summary>
    bool IsResolved { get; }
}
