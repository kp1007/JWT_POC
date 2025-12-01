namespace JwtPoc.Core.Entities;

/// <summary>
/// Audit log entity for tracking all authentication and authorization events
/// Provides comprehensive security audit trail for compliance and investigation
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// Foreign key to User who performed the action
    /// Null for anonymous actions or system-generated events
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Username at the time of the event
    /// Stored to preserve audit trail even if user is deleted
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Type of event
    /// Examples: "Login", "LoginFailed", "Logout", "RoleChanged", "PermissionGranted"
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the event
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// IP address where the action originated
    /// Used for security monitoring and geographic analysis
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string from the request
    /// Helps identify the client application/browser
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// HTTP method of the request (GET, POST, PUT, DELETE)
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Endpoint/path that was accessed
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// HTTP status code of the response
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Severity level of the event
    /// Examples: "Information", "Warning", "Error", "Critical"
    /// </summary>
    public string Severity { get; set; } = "Information";

    /// <summary>
    /// Additional metadata in JSON format
    /// Can store event-specific data without schema changes
    /// Examples: changed fields, old/new values, affected resources
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Indicates whether this event represents a successful action
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Error message if the action failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Duration of the operation in milliseconds
    /// Used for performance monitoring
    /// </summary>
    public long? DurationMs { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public virtual User? User { get; set; }
}
