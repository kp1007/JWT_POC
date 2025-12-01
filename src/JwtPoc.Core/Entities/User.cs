namespace JwtPoc.Core.Entities;

/// <summary>
/// User entity representing an application user with authentication and authorization data
/// Supports multi-factor authentication and role-based access control
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Unique username for login
    /// Must be unique across the system
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// Used for notifications and can be used for login
    /// Must be unique across the system
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password
    /// NEVER store plain text passwords
    /// Uses BCrypt with work factor of 12 for security
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// Can be used for SMS-based 2FA
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Indicates whether the email has been verified
    /// Can be used to restrict access until verification
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Indicates whether the phone number has been verified
    /// Required before enabling SMS-based 2FA
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Indicates whether two-factor authentication is enabled
    /// When enabled, requires additional verification after password
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Secret key for TOTP (Time-based One-Time Password) generation
    /// Stored encrypted, used with authenticator apps
    /// </summary>
    public string? TotpSecret { get; set; }

    /// <summary>
    /// Backup codes for 2FA recovery
    /// Stored as hashed values, used when primary 2FA method unavailable
    /// </summary>
    public List<string> BackupCodes { get; set; } = new();

    /// <summary>
    /// Preferred method for two-factor authentication
    /// Options: None, Authenticator, SMS, Email
    /// </summary>
    public string TwoFactorMethod { get; set; } = "None";

    /// <summary>
    /// Account lockout end time
    /// When set, user cannot login until this time
    /// Used for brute force protection
    /// </summary>
    public DateTime? LockoutEnd { get; set; }

    /// <summary>
    /// Number of failed login attempts since last successful login
    /// Reset to 0 on successful login
    /// Account locked after threshold (typically 5 attempts)
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Indicates whether lockout is enabled for this user
    /// Admin accounts might have this disabled
    /// </summary>
    public bool LockoutEnabled { get; set; } = true;

    /// <summary>
    /// Security stamp that changes when credentials change
    /// Used to invalidate tokens when password is changed
    /// Provides an additional layer of security
    /// </summary>
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Last login timestamp
    /// Used for security auditing and user activity tracking
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// IP address of last login
    /// Used for security monitoring and anomaly detection
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Timestamp of last password change
    /// Can be used to enforce password rotation policies
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; set; }

    /// <summary>
    /// Indicates whether the user account is active
    /// Inactive accounts cannot login
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Roles assigned to this user
    /// Supports many-to-many relationship through UserRole join table
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Refresh tokens issued to this user
    /// Used for JWT token refresh mechanism
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Audit log entries for this user
    /// Tracks all authentication and authorization events
    /// </summary>
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    /// <summary>
    /// Full name of the user
    /// Computed property for convenience
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Checks if the account is currently locked out
    /// </summary>
    public bool IsLockedOut => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
}
