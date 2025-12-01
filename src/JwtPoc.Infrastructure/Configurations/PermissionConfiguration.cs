using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for Permission entity
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(200);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        // Unique index on normalized name
        builder.HasIndex(p => p.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_NormalizedName");

        // Index on category for grouping
        builder.HasIndex(p => p.Category)
            .HasDatabaseName("IX_Permissions_Category");

        // Relationships
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
