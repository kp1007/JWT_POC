using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for RefreshToken entity
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.JwtId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(rt => rt.UsedByIp)
            .HasMaxLength(45);

        builder.Property(rt => rt.RevocationReason)
            .HasMaxLength(200);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);

        builder.Property(rt => rt.PreviousToken)
            .HasMaxLength(256);

        builder.Property(rt => rt.TokenFamily)
            .HasMaxLength(256);

        // Unique index on token
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        // Index on UserId for user token lookups
        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        // Index on JwtId for token family validation
        builder.HasIndex(rt => rt.JwtId)
            .HasDatabaseName("IX_RefreshTokens_JwtId");

        // Index on ExpiresAt for cleanup queries
        builder.HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

        // Index on TokenFamily for family revocation
        builder.HasIndex(rt => rt.TokenFamily)
            .HasDatabaseName("IX_RefreshTokens_TokenFamily");

        // Ignore computed properties
        builder.Ignore(rt => rt.IsActive);
        builder.Ignore(rt => rt.IsUsed);
        builder.Ignore(rt => rt.IsRevoked);
        builder.Ignore(rt => rt.IsExpired);
    }
}
