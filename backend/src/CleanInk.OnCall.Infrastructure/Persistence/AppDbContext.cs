using CleanInk.OnCall.Application.Common.Interfaces;
using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanInk.OnCall.Infrastructure.Persistence;

/// <summary>
/// Multi-tenant EF Core database context.
///
/// Schema-per-tenant strategy:
/// - Each tenant has its own PostgreSQL schema: "tenant_{tenantId:N}".
/// - <see cref="ITenantContext"/> provides the schema name resolved from the JWT claim.
/// - Migrations target the public schema as template; provisioning creates tenant schemas.
///
/// Shared tables (cross-tenant, in "cleanink_shared" schema):
/// - <see cref="Tenants"/> — tenant registry
/// - <see cref="AuditEvents"/> — FHIR AuditEvent (compliance, must survive tenant suspension)
///
/// Per-tenant tables (in "tenant_{id:N}" schema):
/// - All clinical, operational, and billing data
/// </summary>
public sealed class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    // ── Shared (cross-tenant) ─────────────────────────────────────────────

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    // ── Per-tenant ────────────────────────────────────────────────────────

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Call> Calls => Set<Call>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Consent> Consents => Set<Consent>();
    public DbSet<MedicalDocument> MedicalDocuments => Set<MedicalDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Per-tenant tables use the dynamic schema resolved from JWT.
        // Shared tables explicitly override to "cleanink_shared".
        modelBuilder.HasDefaultSchema(_tenantContext.SchemaName);

        modelBuilder.Entity<Tenant>().ToTable("tenants", "cleanink_shared");
        modelBuilder.Entity<AuditEvent>().ToTable("audit_events", "cleanink_shared");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

