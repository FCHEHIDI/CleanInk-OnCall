using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="AuditLog"/> entity.
///
/// The audit_logs table is append-only:
/// - No EF update/delete operations should target this table.
/// - In production, a DB role with INSERT-only permission should be used.
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.ActorId)
            .HasColumnName("actor_id")
            .IsRequired(false);

        builder.Property(a => a.ActorEmail)
            .HasColumnName("actor_email")
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityType)
            .HasColumnName("entity_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .HasColumnName("entity_id")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(a => a.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45) // IPv6 max = 39 chars
            .IsRequired(false);

        builder.Property(a => a.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        // Performance indices for compliance queries.
        builder.HasIndex(a => a.ActorId).HasDatabaseName("ix_audit_actor_id");
        builder.HasIndex(a => new { a.EntityType, a.EntityId }).HasDatabaseName("ix_audit_entity");
        builder.HasIndex(a => a.OccurredAt).HasDatabaseName("ix_audit_occurred_at");
    }
}
