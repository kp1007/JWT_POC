using JwtPoc.Core.DTOs;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for role management operations
/// Handles role CRUD and permission assignments
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Gets a role by ID with permissions
    /// </summary>
    Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles
    /// </summary>
    Task<List<RoleDto>> GetAllRolesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role
    /// </summary>
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request, Guid createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role
    /// Cannot update system roles
    /// </summary>
    Task<RoleDto> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, Guid updatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role
    /// Cannot delete system roles or roles with users
    /// </summary>
    Task DeleteRoleAsync(Guid roleId, Guid deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Grants a permission to a role
    /// </summary>
    Task GrantPermissionAsync(Guid roleId, Guid permissionId, Guid grantedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a permission from a role
    /// </summary>
    Task RevokePermissionAsync(Guid roleId, Guid permissionId, Guid revokedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a role
    /// </summary>
    Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}
