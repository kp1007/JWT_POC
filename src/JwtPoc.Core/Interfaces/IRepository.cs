using System.Linq.Expressions;
using JwtPoc.Core.Entities;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Generic repository interface providing basic CRUD operations
/// Follows repository pattern for data access abstraction
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its unique identifier
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="includeDeleted">Whether to include soft-deleted entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities matching the predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includeDeleted">Whether to include soft-deleted entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching the predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includeDeleted">Whether to include soft-deleted entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    /// <param name="entities">Entities to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    void Update(T entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    /// <param name="entities">Entities to update</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Hard deletes an entity (removes from database)
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(T entity);

    /// <summary>
    /// Soft deletes an entity (sets IsDeleted flag)
    /// </summary>
    /// <param name="entity">Entity to soft delete</param>
    void SoftDelete(T entity);

    /// <summary>
    /// Checks if any entity matches the predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includeDeleted">Whether to include soft-deleted entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any entity matches, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the predicate
    /// </summary>
    /// <param name="predicate">Filter condition (null for all entities)</param>
    /// <param name="includeDeleted">Whether to include soft-deleted entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, CancellationToken cancellationToken = default);
}
