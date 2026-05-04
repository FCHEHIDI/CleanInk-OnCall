using CleanInk.OnCall.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanInk.OnCall.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for the <see cref="User"/> aggregate.
/// User.Name is a HumanName owned value object, flattened to name_family / name_given columns.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.TenantId).IsRequired();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        // HumanName owned type — flattened to name_family and name_given columns.
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.Family)
                .HasColumnName("name_family")
                .IsRequired()
                .HasMaxLength(100);

            name.Property(n => n.Given)
                .HasColumnName("name_given")
                .IsRequired()
                .HasMaxLength(500)
                .HasConversion(
                    arr => string.Join("|", arr),
                    raw => raw.Split('|', StringSplitOptions.RemoveEmptyEntries));
        });

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder.HasIndex(u => u.TenantId);
    }
}
