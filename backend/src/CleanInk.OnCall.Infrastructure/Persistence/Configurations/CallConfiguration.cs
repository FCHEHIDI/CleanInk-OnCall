using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for the <see cref="Call"/> aggregate.
/// </summary>
public sealed class CallConfiguration : IEntityTypeConfiguration<Call>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Call> builder)
    {
        builder.ToTable("calls");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.CustomerId).IsRequired();
        builder.Property(c => c.PatientId);
        builder.Property(c => c.OperatorId);

        builder.Property(c => c.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(5000);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.AiTriageTag)
            .HasMaxLength(100);

        builder.Property(c => c.AiSummary)
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.HasIndex(c => c.CustomerId);
        builder.HasIndex(c => c.PatientId);
        builder.HasIndex(c => c.OperatorId);
        builder.HasIndex(c => c.Status);
    }
}
