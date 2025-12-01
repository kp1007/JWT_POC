using System.ComponentModel.DataAnnotations;

namespace JwtPoc.Core.DTOs;

/// <summary>
/// DTO representing role information for API responses
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public int UserCount { get; set; }
}

/// <summary>
/// Request DTO for creating a new role
/// </summary>
public class CreateRoleRequest
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }

    [Range(1, 1000, ErrorMessage = "Level must be between 1 and 1000")]
    public int Level { get; set; } = 1;

    public List<Guid> PermissionIds { get; set; } = new();
}

/// <summary>
/// Request DTO for updating an existing role
/// </summary>
public class UpdateRoleRequest
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }

    [Range(1, 1000, ErrorMessage = "Level must be between 1 and 1000")]
    public int Level { get; set; }

    public bool IsActive { get; set; }

    public List<Guid> PermissionIds { get; set; } = new();
}

/// <summary>
/// Request DTO for assigning roles to a user
/// </summary>
public class AssignRolesRequest
{
    [Required(ErrorMessage = "At least one role must be specified")]
    [MinLength(1, ErrorMessage = "At least one role must be specified")]
    public List<Guid> RoleIds { get; set; } = new();

    /// <summary>
    /// Optional expiration date for temporary role assignments
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
