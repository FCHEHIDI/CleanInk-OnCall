using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanInk.OnCall.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the CleanInk OnCall application.
/// Contains DbSets for all three aggregates: <see cref="Call"/>, <see cref="Invoice"/>, <see cref="User"/>.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="AppDbContext"/> with the given options.
    /// </summary>
    /// <param name="options">EF Core context options (connection string, provider, etc.).</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Gets or sets the calls table.</summary>
    public DbSet<Call> Calls => Set<Call>();

    /// <summary>Gets or sets the invoices table.</summary>
    public DbSet<Invoice> Invoices => Set<Invoice>();

    /// <summary>Gets or sets the users table.</summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Gets or sets the patients table (healthcare aggregate).</summary>
    public DbSet<Patient> Patients => Set<Patient>();

    /// <summary>Gets or sets the audit log table (HDS compliance).</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
