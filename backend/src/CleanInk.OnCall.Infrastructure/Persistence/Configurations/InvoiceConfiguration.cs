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

        builder.Property(i => i.CustomerId).IsRequired();
        builder.Property(i => i.CallId);

        builder.Property(i => i.Reference)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(i => i.Reference).IsUnique();

        builder.Property(i => i.AmountCents).IsRequired();

        builder.Property(i => i.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("EUR");

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.DueDate).IsRequired();
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.UpdatedAt).IsRequired();

        builder.HasIndex(i => i.CustomerId);
        builder.HasIndex(i => i.Status);
    }
}
