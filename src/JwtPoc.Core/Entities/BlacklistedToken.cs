namespace JwtPoc.Core.Entities;

/// <summary>
/// Blacklisted token entity for tracking invalidated JWT access tokens
/// Used for immediate token invalidation on logout or security events
/// Provides defense against token replay attacks
/// </summary>
public class BlacklistedToken : BaseEntity
{
    /// <summary>
    /// The JWT ID (jti claim) of the blacklisted token
    /// Unique identifier for each JWT
    /// </summary>
    public string JwtId { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to User who owned this token
    /// Used for audit and bulk revocation
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Timestamp when the original token expires
    /// After this time, the blacklist entry can be safely removed
    /// </summary>
    public DateTime TokenExpiresAt { get; set; }

    /// <summary>
    /// Timestamp when this token was blacklisted
    /// </summary>
    public DateTime BlacklistedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Reason for blacklisting
    /// Examples: "User logout", "Password changed", "Account compromised", "Admin revocation"
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// IP address from which the blacklist action was initiated
    /// Used for security monitoring
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User ID who blacklisted this token
    /// Null if it was the token owner or system
    /// </summary>
    public Guid? BlacklistedBy { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Checks if this blacklist entry is still relevant
    /// Entries can be purged after token expiration
    /// </summary>
    public bool IsStillRelevant => DateTime.UtcNow < TokenExpiresAt;
}
