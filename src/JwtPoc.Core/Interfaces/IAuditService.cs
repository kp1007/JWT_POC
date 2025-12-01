using JwtPoc.Core.DTOs;
using JwtPoc.Core.Enums;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for audit logging operations
/// Tracks security and business events for compliance and investigation
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit event
    /// </summary>
    /// <param name="eventType">Type of event</param>
    /// <param name="description">Event description</param>
    /// <param name="userId">User who performed the action (null for anonymous)</param>
    /// <param name="username">Username at time of event</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userAgent">Client user agent</param>
    /// <param name="severity">Event severity</param>
    /// <param name="isSuccessful">Whether the action succeeded</param>
    /// <param name="metadata">Additional event data in JSON format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogEventAsync(
        AuditEventType eventType,
        string description,
        Guid? userId = null,
        string? username = null,
        string? ipAddress = null,
        string? userAgent = null,
        AuditSeverity severity = AuditSeverity.Information,
        bool isSuccessful = true,
        string? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a successful login event
    /// </summary>
    Task LogLoginSuccessAsync(Guid userId, string username, string ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a failed login attempt
    /// </summary>
    Task LogLoginFailureAsync(string usernameOrEmail, string ipAddress, string? userAgent, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a logout event
    /// </summary>
    Task LogLogoutAsync(Guid userId, string username, string ipAddress, bool logoutAllDevices, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a password change event
    /// </summary>
    Task LogPasswordChangeAsync(Guid userId, string username, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a role assignment or removal
    /// </summary>
    Task LogRoleChangeAsync(Guid userId, string username, string roleName, bool isAssignment, Guid? changedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs suspicious activity
    /// </summary>
    Task LogSuspiciousActivityAsync(string description, Guid? userId, string? username, string? ipAddress, string? metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves audit logs with filtering
    /// </summary>
    Task<PaginatedResponse<AuditLogDto>> GetAuditLogsAsync(AuditLogQuery query, CancellationToken cancellationToken = default);
}
