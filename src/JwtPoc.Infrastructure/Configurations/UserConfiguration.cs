using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for User entity
/// Defines database schema, relationships, and constraints
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.TotpSecret)
            .HasMaxLength(256);

        builder.Property(u => u.TwoFactorMethod)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("None");

        builder.Property(u => u.BackupCodes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(1000);

        builder.Property(u => u.SecurityStamp)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.LastLoginIp)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.LockoutEnabled)
            .HasDefaultValue(true);

        builder.Property(u => u.AccessFailedCount)
            .HasDefaultValue(0);

        // Unique indexes
        // These ensure data integrity and improve query performance
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Additional indexes for common queries
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => new { u.Email, u.IsDeleted })
            .HasDatabaseName("IX_Users_Email_IsDeleted");

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AuditLogs)
            .WithOne(al => al.User)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore computed properties
        builder.Ignore(u => u.FullName);
        builder.Ignore(u => u.IsLockedOut);
    }
}
