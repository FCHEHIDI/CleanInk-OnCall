using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for the <see cref="Invoice"/> aggregate.
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.PatientId).IsRequired();
        builder.Property(i => i.EncounterId).IsRequired(false);
        builder.Property(i => i.PractitionerId).IsRequired(false);

        builder.Property(i => i.Reference)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(i => i.Reference).IsUnique();

        builder.Property(i => i.AmountCents).IsRequired();
        builder.Property(i => i.VatCents).IsRequired();

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.PaymentMethod).HasMaxLength(100).IsRequired(false);
        builder.Property(i => i.ThirdPartyPayerClaimId).IsRequired(false);

        builder.Property(i => i.IssuedAt).IsRequired();
        builder.Property(i => i.DueAt).IsRequired();
        builder.Property(i => i.PaidAt).IsRequired(false);
        builder.Property(i => i.UpdatedAt).IsRequired();

        builder.HasIndex(i => i.PatientId);
        builder.HasIndex(i => i.Status);
    }
}
