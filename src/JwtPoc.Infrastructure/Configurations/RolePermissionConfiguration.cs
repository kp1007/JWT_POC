using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for RolePermission join table
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => rp.Id);

        builder.Property(rp => rp.GrantedAt)
            .IsRequired();

        builder.Property(rp => rp.IsActive)
            .HasDefaultValue(true);

        // Composite index on RoleId and PermissionId
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_RoleId_PermissionId");

        // Index on PermissionId for reverse lookups
        builder.HasIndex(rp => rp.PermissionId)
            .HasDatabaseName("IX_RolePermissions_PermissionId");
    }
}
