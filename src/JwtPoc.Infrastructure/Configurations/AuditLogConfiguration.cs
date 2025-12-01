using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JwtPoc.Core.Entities;

namespace JwtPoc.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for AuditLog entity
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.Username)
            .HasMaxLength(50);

        builder.Property(al => al.EventType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(al => al.IpAddress)
            .HasMaxLength(45);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.Property(al => al.HttpMethod)
            .HasMaxLength(10);

        builder.Property(al => al.Endpoint)
            .HasMaxLength(200);

        builder.Property(al => al.Severity)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Information");

        builder.Property(al => al.Metadata)
            .HasColumnType("nvarchar(max)"); // JSON data

        builder.Property(al => al.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(al => al.IsSuccessful)
            .HasDefaultValue(true);

        // Indexes for common query patterns
        builder.HasIndex(al => al.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.HasIndex(al => al.EventType)
            .HasDatabaseName("IX_AuditLogs_EventType");

        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("IX_AuditLogs_CreatedAt");

        builder.HasIndex(al => al.Severity)
            .HasDatabaseName("IX_AuditLogs_Severity");

        builder.HasIndex(al => new { al.UserId, al.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_UserId_CreatedAt");

        // Composite index for common filters
        builder.HasIndex(al => new { al.EventType, al.CreatedAt })
            .HasDatabaseName("IX_AuditLogs_EventType_CreatedAt");
    }
}
