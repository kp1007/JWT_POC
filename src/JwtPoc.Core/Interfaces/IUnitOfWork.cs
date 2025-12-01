using JwtPoc.Core.Entities;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Unit of Work pattern interface
/// Coordinates multiple repository operations into a single transaction
/// Ensures data consistency and provides atomic commits
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for User entities
    /// </summary>
    IRepository<User> Users { get; }

    /// <summary>
    /// Repository for Role entities
    /// </summary>
    IRepository<Role> Roles { get; }

    /// <summary>
    /// Repository for Permission entities
    /// </summary>
    IRepository<Permission> Permissions { get; }

    /// <summary>
    /// Repository for UserRole entities
    /// </summary>
    IRepository<UserRole> UserRoles { get; }

    /// <summary>
    /// Repository for RolePermission entities
    /// </summary>
    IRepository<RolePermission> RolePermissions { get; }

    /// <summary>
    /// Repository for RefreshToken entities
    /// </summary>
    IRepository<RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Repository for AuditLog entities
    /// </summary>
    IRepository<AuditLog> AuditLogs { get; }

    /// <summary>
    /// Repository for BlacklistedToken entities
    /// </summary>
    IRepository<BlacklistedToken> BlacklistedTokens { get; }

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
