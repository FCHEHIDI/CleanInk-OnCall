using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Shared.Fhir;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Consent"/> aggregate.
/// ValidityPeriod (FHIR Period record) stored as JSONB.
/// </summary>
public sealed class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.ToTable("consents");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(c => c.CollectedByUserId).HasColumnName("collected_by_user_id").IsRequired();

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Scope)
            .HasColumnName("scope")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ConsentedAt).HasColumnName("consented_at").IsRequired();
        builder.Property(c => c.WithdrawnAt).HasColumnName("withdrawn_at").IsRequired(false);

        builder.Property(c => c.ValidityPeriod)
            .HasColumnName("validity_period")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => JsonSerializer.Deserialize<Period>(raw, _json)!);

        builder.Property(c => c.CollectionMethod)
            .HasColumnName("collection_method")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.PatientId);
        builder.HasIndex(c => c.Status);
    }
}
