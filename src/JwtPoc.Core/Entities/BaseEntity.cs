namespace JwtPoc.Core.Entities;

/// <summary>
/// Base entity class providing common properties for all entities
/// Implements soft delete pattern and audit tracking
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the entity was created
    /// Set automatically during entity creation
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated
    /// Updated automatically on entity modification
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag - when true, entity is considered deleted
    /// Allows for data recovery and audit trail maintenance
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Timestamp when the entity was soft deleted
    /// Null if entity is not deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
