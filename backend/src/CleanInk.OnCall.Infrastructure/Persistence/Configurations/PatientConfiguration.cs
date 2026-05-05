using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Shared.Fhir;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Patient"/> aggregate.
/// FHIR R4 — Names and ContactPoints stored as JSON columns.
/// NirEncrypted stores already-encrypted NIR value.
/// </summary>
public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Safely deserializes a JSONB column into a list.
    /// Guards against the legacy default value '{}' (empty JSON object) that EF
    /// migrations can write instead of '[]' — avoids a JsonException at load time.
    /// </summary>
    private static List<T> SafeList<T>(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return [];
        var trimmed = raw.TrimStart();
        if (!trimmed.StartsWith('[')) return [];
        return JsonSerializer.Deserialize<List<T>>(raw, _json) ?? [];
    }

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        // FHIR Lists stored as JSONB. Default is '[]' (empty array, not object).
        builder.Property(p => p.Names)
            .HasColumnName("names")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => SafeList<HumanName>(raw));

        builder.Property(p => p.Identifiers)
            .HasColumnName("identifiers")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => SafeList<FhirIdentifier>(raw));

        builder.Property(p => p.ContactPoints)
            .HasColumnName("contact_points")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, _json),
                raw => SafeList<ContactPoint>(raw));

        builder.Property(p => p.Gender)
            .HasColumnName("gender")
            .HasMaxLength(20);

        builder.Property(p => p.DateOfBirth)
            .IsRequired()
            .HasColumnName("date_of_birth");

        builder.Property(p => p.NirEncrypted)
            .HasColumnName("nir_encrypted")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.ConsentGiven)
            .IsRequired()
            .HasColumnName("consent_given");

        builder.Property(p => p.ConsentGivenAt)
            .HasColumnName("consent_given_at")
            .IsRequired(false);

        builder.Property(p => p.IsPseudonymized)
            .IsRequired()
            .HasColumnName("is_pseudonymized")
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasIndex(p => p.ConsentGiven);
    }
}
