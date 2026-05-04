using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="AuditEvent"/> entity.
/// Append-only — lives in cleanink_shared schema.
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("audit_events", "cleanink_shared");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(a => a.ActorId)
            .HasColumnName("actor_id")
            .IsRequired(false);

        builder.Property(a => a.ActorEmail)
            .HasColumnName("actor_email")
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(a => a.ActorRole)
            .HasColumnName("actor_role")
            .HasMaxLength(50);

        builder.Property(a => a.EventType)
            .HasColumnName("event_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ResourceType)
            .HasColumnName("resource_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ResourceId)
            .HasColumnName("resource_id")
            .IsRequired(false);

        builder.Property(a => a.Outcome)
            .HasColumnName("outcome")
            .HasMaxLength(20);

        builder.Property(a => a.IsEmergencyAccess)
            .HasColumnName("is_emergency_access");

        builder.Property(a => a.EmergencyJustification)
            .HasColumnName("emergency_justification")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(a => a.Details)
            .HasColumnName("details")
            .IsRequired(false);

        builder.Property(a => a.RecordedAt)
            .HasColumnName("recorded_at")
            .IsRequired();

        builder.HasIndex(a => a.ActorId).HasDatabaseName("ix_audit_actor_id");
        builder.HasIndex(a => new { a.ResourceType, a.ResourceId }).HasDatabaseName("ix_audit_resource");
        builder.HasIndex(a => a.RecordedAt).HasDatabaseName("ix_audit_recorded_at");
        builder.HasIndex(a => a.TenantId).HasDatabaseName("ix_audit_tenant_id");
    }
}
