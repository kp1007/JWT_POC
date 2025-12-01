namespace JwtPoc.Core.Entities;

/// <summary>
/// Join table entity for many-to-many relationship between Roles and Permissions
/// Tracks when and by whom permissions were granted to roles
/// </summary>
public class RolePermission : BaseEntity
{
    /// <summary>
    /// Foreign key to Role entity
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Foreign key to Permission entity
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Timestamp when this permission was granted to the role
    /// </summary>
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who granted this permission
    /// Used for audit trail
    /// Null if granted during system initialization
    /// </summary>
    public Guid? GrantedBy { get; set; }

    /// <summary>
    /// Indicates whether this permission grant is currently active
    /// Can be used to temporarily revoke a permission without removing it
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Navigation property to Role
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Navigation property to Permission
    /// </summary>
    public virtual Permission Permission { get; set; } = null!;
}
