namespace JwtPoc.Core.Entities;

/// <summary>
/// Refresh token entity for JWT token refresh mechanism
/// Implements token rotation and family validation for enhanced security
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// The actual refresh token string
    /// Cryptographically secure random string (typically 64+ bytes)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to User who owns this token
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// JWT ID (jti) of the access token this refresh token was issued with
    /// Used for token family validation
    /// </summary>
    public string JwtId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when this token expires
    /// After expiration, token cannot be used for refresh
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Timestamp when this token was used for refresh
    /// Null if not yet used
    /// Used to detect token reuse attacks
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Timestamp when this token was revoked
    /// Null if not revoked
    /// Revoked tokens cannot be used
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Reason for token revocation
    /// Examples: "User logout", "Password changed", "Security concern"
    /// </summary>
    public string? RevocationReason { get; set; }

    /// <summary>
    /// IP address where token was created
    /// Used for security monitoring
    /// </summary>
    public string CreatedByIp { get; set; } = string.Empty;

    /// <summary>
    /// IP address where token was used/revoked
    /// Used for security monitoring and anomaly detection
    /// </summary>
    public string? UsedByIp { get; set; }

    /// <summary>
    /// Token that replaced this one after refresh
    /// Used for token family tracking
    /// Null if this is the current active token in the family
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Previous token in the refresh chain
    /// Used for token family validation
    /// Null if this is the first token in the family
    /// </summary>
    public string? PreviousToken { get; set; }

    /// <summary>
    /// Root token of the token family
    /// All tokens in a refresh chain share the same root
    /// Used to revoke entire token families
    /// </summary>
    public string? TokenFamily { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Checks if the token is currently active (not used, not revoked, not expired)
    /// </summary>
    public bool IsActive => !IsUsed && !IsRevoked && !IsExpired;

    /// <summary>
    /// Checks if the token has been used for refresh
    /// </summary>
    public bool IsUsed => UsedAt.HasValue;

    /// <summary>
    /// Checks if the token has been revoked
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>
    /// Checks if the token has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
