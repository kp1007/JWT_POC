namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for managing blacklisted tokens
/// Provides immediate token invalidation for logout and security events
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// Blacklists a token by its JTI
    /// </summary>
    /// <param name="jti">JWT ID to blacklist</param>
    /// <param name="userId">User who owns the token</param>
    /// <param name="expiresAt">When the original token expires</param>
    /// <param name="reason">Reason for blacklisting</param>
    /// <param name="ipAddress">IP address of the action</param>
    /// <param name="blacklistedBy">User ID who blacklisted the token (null if self)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BlacklistTokenAsync(
        string jti,
        Guid userId,
        DateTime expiresAt,
        string reason,
        string? ipAddress = null,
        Guid? blacklistedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Blacklists all tokens for a user
    /// Used during logout from all devices or password change
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="reason">Reason for blacklisting</param>
    /// <param name="ipAddress">IP address of the action</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BlacklistAllUserTokensAsync(Guid userId, string reason, string? ipAddress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a token is blacklisted
    /// </summary>
    /// <param name="jti">JWT ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if blacklisted, false otherwise</returns>
    Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes expired blacklist entries
    /// Should be called periodically to clean up the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CleanupExpiredEntriesAsync(CancellationToken cancellationToken = default);
}
