using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Tenant aggregate root — represents a healthcare organisation (hôpital, clinique, cabinet de groupe).
///
/// Multi-tenant architecture:
/// - Each tenant owns a dedicated PostgreSQL schema: <c>tenant_{Id:N}</c>.
/// - Data in the tenant schema is completely isolated from other tenants.
/// - The Tenant entity itself lives in the <c>cleanink_shared</c> schema.
///
/// Provisioning lifecycle:
/// 1. Tenant record created (status = Provisioning).
/// 2. TenantProvisioningService creates the DB schema + runs EF migrations.
/// 3. Initial Admin user created in the tenant schema.
/// 4. Status set to Active.
/// </summary>
public sealed class Tenant : Entity<Guid>
{
    private Tenant() { }

    /// <summary>Human-readable organisation name (e.g. "Hôpital Saint-Antoine").</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Unique subdomain / slug for routing (e.g. "saint-antoine").
    /// Used as the URL prefix: https://saint-antoine.cleanink.app
    /// Must be lowercase alphanumeric + hyphens only.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>FINESS number of the healthcare facility (14 digits). Nullable for private practices.</summary>
    public string? FinessNumber { get; private set; }

    /// <summary>Current lifecycle status of the tenant.</summary>
    public TenantStatus Status { get; private set; }

    /// <summary>UTC timestamp of tenant creation.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>PostgreSQL schema name: <c>tenant_{Id:N}</c>.</summary>
    public string SchemaName => $"tenant_{Id:N}";

    /// <summary>
    /// Factory method — creates a new tenant in Provisioning state.
    /// Raises <see cref="TenantCreatedEvent"/>.
    /// </summary>
    /// <param name="name">Organisation name (non-empty).</param>
    /// <param name="slug">Unique URL slug (lowercase, alphanumeric + hyphens).</param>
    /// <param name="finessNumber">Optional FINESS number (14 digits).</param>
    public static Result<Tenant> Create(string name, string slug, string? finessNumber = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation(nameof(Name), "Tenant name is required.");

        if (string.IsNullOrWhiteSpace(slug) || !IsValidSlug(slug))
            return Error.Validation(nameof(Slug),
                "Slug must be lowercase alphanumeric characters and hyphens only (e.g. 'hopital-saint-antoine').");

        if (finessNumber is not null && finessNumber.Length != 14)
            return Error.Validation(nameof(FinessNumber), "FINESS number must be 14 digits.");

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            FinessNumber = finessNumber,
            Status = TenantStatus.Provisioning,
            CreatedAt = DateTime.UtcNow,
        };

        tenant.RaiseDomainEvent(new TenantCreatedEvent(tenant.Id, tenant.Name, tenant.Slug, DateTime.UtcNow));

        return tenant;
    }

    /// <summary>Marks the tenant as fully provisioned and active.</summary>
    public void Activate()
    {
        if (Status != TenantStatus.Provisioning)
            throw new InvalidOperationException($"Cannot activate a tenant in '{Status}' state.");

        Status = TenantStatus.Active;
        RaiseDomainEvent(new TenantActivatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>Suspends the tenant (e.g. unpaid subscription).</summary>
    public void Suspend()
    {
        if (Status == TenantStatus.Suspended)
            return;

        Status = TenantStatus.Suspended;
    }

    private static bool IsValidSlug(string slug) =>
        !string.IsNullOrWhiteSpace(slug) &&
        slug.Length <= 63 &&
        System.Text.RegularExpressions.Regex.IsMatch(slug, @"^[a-z0-9][a-z0-9\-]*[a-z0-9]$");
}

/// <summary>Lifecycle states for a <see cref="Tenant"/>.</summary>
public enum TenantStatus
{
    /// <summary>Schema is being created — not yet usable.</summary>
    Provisioning = 0,

    /// <summary>Fully provisioned and accepting traffic.</summary>
    Active = 1,

    /// <summary>Temporarily suspended (billing, compliance).</summary>
    Suspended = 2,
}
