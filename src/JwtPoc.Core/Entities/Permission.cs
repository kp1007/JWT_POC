namespace JwtPoc.Core.Entities;

/// <summary>
/// Permission entity representing a granular access right in the system
/// Permissions are assigned to roles, which are then assigned to users
/// Follows the principle of least privilege
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Unique name of the permission (e.g., "users.read", "users.write", "reports.delete")
    /// Uses dot notation for hierarchical organization
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Normalized name for case-insensitive comparisons
    /// Stored in uppercase for consistent lookups
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this permission allows
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Category or module this permission belongs to
    /// Used for grouping permissions in UI (e.g., "Users", "Reports", "Settings")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this permission is active
    /// Inactive permissions are not enforced
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Roles that have been granted this permission
    /// Supports many-to-many relationship through RolePermission join table
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
