using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="MedicalDocument"/> aggregate.
/// Binary content is stored externally (blob storage) — only metadata is mapped here.
/// </summary>
public sealed class MedicalDocumentConfiguration : IEntityTypeConfiguration<MedicalDocument>
{
    public void Configure(EntityTypeBuilder<MedicalDocument> builder)
    {
        builder.ToTable("medical_documents");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(d => d.EncounterId).HasColumnName("encounter_id").IsRequired(false);
        builder.Property(d => d.AuthoredByUserId).HasColumnName("authored_by_user_id").IsRequired();

        builder.Property(d => d.DocumentType)
            .HasColumnName("document_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.MimeType)
            .HasColumnName("mime_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.SizeBytes).HasColumnName("size_bytes").IsRequired();

        builder.Property(d => d.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(d => d.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(d => d.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false).IsRequired();

        builder.HasIndex(d => d.PatientId);
        builder.HasIndex(d => new { d.PatientId, d.IsDeleted });
    }
}
