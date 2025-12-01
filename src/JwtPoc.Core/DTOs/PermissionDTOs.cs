using System.ComponentModel.DataAnnotations;

namespace JwtPoc.Core.DTOs;

/// <summary>
/// DTO representing permission information for API responses
/// </summary>
public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// Request DTO for creating a new permission
/// </summary>
public class CreatePermissionRequest
{
    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Permission name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-z]+(\.[a-z]+)*$", ErrorMessage = "Permission name must use lowercase and dot notation (e.g., 'users.read')")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for updating an existing permission
/// </summary>
public class UpdatePermissionRequest
{
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
