using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Shared.Fhir;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Encounter"/> aggregate.
/// FHIR CodeableConcept and Period stored as JSONB columns.
/// </summary>
public sealed class EncounterConfiguration : IEntityTypeConfiguration<Encounter>
{
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<Encounter> builder)
    {
        builder.ToTable("encounters");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Identifiers)
            .HasColumnName("identifiers")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => JsonSerializer.Deserialize<List<FhirIdentifier>>(raw, _json) ?? new());

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.EncounterClass)
            .HasColumnName("encounter_class")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => JsonSerializer.Deserialize<CodeableConcept>(raw, _json)!);

        builder.Property(e => e.Type)
            .HasColumnName("encounter_type")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, _json),
                raw => raw == null ? null : JsonSerializer.Deserialize<CodeableConcept>(raw, _json));

        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.PractitionerId).HasColumnName("practitioner_id").IsRequired();

        builder.Property(e => e.Period)
            .HasColumnName("period")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => JsonSerializer.Deserialize<Period>(raw, _json)!);

        builder.Property(e => e.ReasonText)
            .HasColumnName("reason_text")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(e => e.ReasonCode)
            .HasColumnName("reason_code")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, _json),
                raw => raw == null ? null : JsonSerializer.Deserialize<CodeableConcept>(raw, _json));

        builder.Property(e => e.ClinicalNote)
            .HasColumnName("clinical_note")
            .IsRequired(false);

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.PractitionerId);
        builder.HasIndex(e => e.Status);
    }
}
