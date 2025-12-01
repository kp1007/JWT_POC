namespace JwtPoc.Core.Enums;

/// <summary>
/// Enumeration of audit event types
/// Used for categorizing security and business events
/// </summary>
public enum AuditEventType
{
    // Authentication Events
    Login,
    LoginFailed,
    Logout,
    TokenRefreshed,
    TokenRevoked,

    // Registration Events
    UserRegistered,
    EmailVerified,
    PhoneNumberVerified,

    // 2FA Events
    TwoFactorEnabled,
    TwoFactorDisabled,
    TwoFactorMethodChanged,
    TwoFactorCodeGenerated,
    TwoFactorSuccess,
    TwoFactorFailed,

    // Password Events
    PasswordChanged,
    PasswordResetRequested,
    PasswordResetCompleted,

    // Account Events
    AccountLocked,
    AccountUnlocked,
    AccountDeactivated,
    AccountActivated,
    AccountDeleted,

    // Role and Permission Events
    RoleAssigned,
    RoleRemoved,
    RoleCreated,
    RoleUpdated,
    RoleDeleted,
    PermissionGranted,
    PermissionRevoked,

    // Security Events
    SecurityStampChanged,
    SuspiciousActivity,
    MultipleFailedAttempts,
    TokenReplayDetected,

    // Profile Events
    ProfileUpdated,
    EmailChanged,
    PhoneNumberChanged,

    // Administrative Events
    UserCreatedByAdmin,
    UserUpdatedByAdmin,
    UserDeletedByAdmin,
    BulkOperation,

    // System Events
    SystemError,
    ConfigurationChanged,
    DataSeeded
}
