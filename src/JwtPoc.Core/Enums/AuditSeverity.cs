namespace JwtPoc.Core.Enums;

/// <summary>
/// Severity levels for audit events
/// Used for filtering and alerting
/// </summary>
public enum AuditSeverity
{
    /// <summary>
    /// Informational events - normal operations
    /// </summary>
    Information = 0,

    /// <summary>
    /// Warning events - unusual but not necessarily harmful
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error events - operation failures
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical events - security concerns requiring immediate attention
    /// </summary>
    Critical = 3
}
