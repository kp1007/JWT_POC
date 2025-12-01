using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for UserRole join table
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.AssignedAt)
            .IsRequired();

        builder.Property(ur => ur.IsActive)
            .HasDefaultValue(true);

        // Composite index on UserId and RoleId
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_UserId_RoleId");

        // Index on RoleId for reverse lookups
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRoles_RoleId");

        // Index on expiration for cleanup queries
        builder.HasIndex(ur => ur.ExpiresAt)
            .HasDatabaseName("IX_UserRoles_ExpiresAt");

        // Ignore computed properties
        builder.Ignore(ur => ur.IsValid);
    }
}
