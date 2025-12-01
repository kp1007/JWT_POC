namespace JwtPoc.Core.Entities;

/// <summary>
/// Role entity representing a security role in the RBAC system
/// Supports hierarchical roles and permission-based authorization
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Unique name of the role (e.g., "Admin", "Manager", "User")
    /// Used in authorization policies and role checks
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Normalized name for case-insensitive comparisons
    /// Stored in uppercase for consistent lookups
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the role
    /// Explains the purpose and capabilities of the role
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Hierarchical level of the role
    /// Higher values indicate more privileged roles
    /// Used for role comparison and hierarchy enforcement
    /// Example: Admin=100, Manager=50, User=1
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Indicates whether this is a system role
    /// System roles cannot be deleted or modified
    /// Examples: SuperAdmin, Admin, User
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Indicates whether this role is active
    /// Inactive roles cannot be assigned to users
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Users assigned to this role
    /// Supports many-to-many relationship through UserRole join table
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Permissions granted to this role
    /// Supports many-to-many relationship through RolePermission join table
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
