using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for BlacklistedToken entity
/// </summary>
public class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
{
    public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
    {
        builder.ToTable("BlacklistedTokens");

        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.JwtId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(bt => bt.TokenExpiresAt)
            .IsRequired();

        builder.Property(bt => bt.BlacklistedAt)
            .IsRequired();

        builder.Property(bt => bt.Reason)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bt => bt.IpAddress)
            .HasMaxLength(45);

        // Unique index on JwtId - each token can only be blacklisted once
        builder.HasIndex(bt => bt.JwtId)
            .IsUnique()
            .HasDatabaseName("IX_BlacklistedTokens_JwtId");

        // Index on UserId for user-specific queries
        builder.HasIndex(bt => bt.UserId)
            .HasDatabaseName("IX_BlacklistedTokens_UserId");

        // Index on TokenExpiresAt for cleanup queries
        builder.HasIndex(bt => bt.TokenExpiresAt)
            .HasDatabaseName("IX_BlacklistedTokens_TokenExpiresAt");

        // Ignore computed properties
        builder.Ignore(bt => bt.IsStillRelevant);
    }
}
