using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Patient"/> aggregate.
///
/// HDS compliance:
/// - NIR (column nir_number) and Phone (column phone_number) are tagged as sensitive.
///   In a production HDS deployment, these columns would use PostgreSQL pgcrypto
///   or transparent data encryption. For now, an application-level AES-256 value
///   converter is the target (tracked as TODO).
/// </summary>
public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("last_name");

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("first_name");

        builder.Property(p => p.DateOfBirth)
            .IsRequired()
            .HasColumnName("date_of_birth");

        // NIR is stored as its string value. TODO: add AES-256 value converter.
        builder.Property(p => p.Nir)
            .HasColumnName("nir_number")
            .HasMaxLength(20)
            .HasConversion(
                nir => nir == null ? null : nir.Value,
                raw => raw == null ? null : Domain.ValueObjects.NirNumber.Create(raw).Value);

        // Phone stored as E.164 string. TODO: add AES-256 value converter.
        builder.Property(p => p.Phone)
            .HasColumnName("phone_number")
            .HasMaxLength(20)
            .HasConversion(
                phone => phone == null ? null : phone.Value,
                raw => raw == null ? null : Domain.ValueObjects.PhoneNumber.Create(raw).Value);

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .HasConversion(
                email => email == null ? null : email.Value,
                raw => raw == null ? null : Domain.ValueObjects.EmailAddress.Create(raw).Value);

        builder.Property(p => p.Consent)
            .IsRequired()
            .HasColumnName("consent_status")
            .HasConversion<string>();

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(p => p.IsArchived)
            .IsRequired()
            .HasColumnName("is_archived")
            .HasDefaultValue(false);

        // Index for name search performance.
        builder.HasIndex(p => p.LastName).HasDatabaseName("ix_patients_last_name");

        // Unique index on NIR (when not null) — prevents duplicate registration.
        builder.HasIndex(p => p.Nir)
            .IsUnique()
            .HasFilter("nir_number IS NOT NULL")
            .HasDatabaseName("ix_patients_nir_unique");
    }
}
