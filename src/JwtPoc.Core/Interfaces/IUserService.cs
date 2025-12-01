using JwtPoc.Core.DTOs;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for user management operations
/// Handles user CRUD, profile management, and role assignments
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of users with filtering
    /// </summary>
    Task<UserListDto> GetUsersAsync(UserListQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user (admin operation)
    /// </summary>
    Task<UserDto> CreateUserAsync(CreateUserRequest request, Guid createdBy, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user's profile (admin operation)
    /// </summary>
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, Guid updatedBy, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates current user's profile
    /// </summary>
    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's email
    /// Requires password verification and email confirmation
    /// </summary>
    Task UpdateEmailAsync(Guid userId, UpdateEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user
    /// </summary>
    Task DeleteUserAsync(Guid userId, Guid deletedBy, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns roles to a user
    /// Handles dynamic role change detection
    /// </summary>
    Task AssignRolesAsync(Guid userId, AssignRolesRequest request, Guid assignedBy, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task RemoveRoleAsync(Guid userId, Guid roleId, Guid removedBy, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a user (aggregated from their roles)
    /// </summary>
    Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks a user account
    /// </summary>
    Task LockUserAsync(Guid userId, DateTime lockoutEnd, string reason, Guid lockedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks a user account
    /// </summary>
    Task UnlockUserAsync(Guid userId, Guid unlockedBy, CancellationToken cancellationToken = default);
}
