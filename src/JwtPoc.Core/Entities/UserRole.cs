namespace JwtPoc.Core.Entities;

/// <summary>
/// Join table entity for many-to-many relationship between Users and Roles
/// Tracks when and by whom roles were assigned
/// </summary>
public class UserRole : BaseEntity
{
    /// <summary>
    /// Foreign key to User entity
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to Role entity
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Timestamp when this role was assigned to the user
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who assigned this role
    /// Used for audit trail
    /// Null if assigned during user creation
    /// </summary>
    public Guid? AssignedBy { get; set; }

    /// <summary>
    /// Optional expiration date for temporary role assignments
    /// Null means the role assignment doesn't expire
    /// Useful for temporary elevated permissions
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Indicates whether this role assignment is currently active
    /// Can be used to temporarily suspend a role without removing it
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation property to Role
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Checks if this role assignment is currently valid
    /// Considers both active status and expiration
    /// </summary>
    public bool IsValid => IsActive && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow);
}
