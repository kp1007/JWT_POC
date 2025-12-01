namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for two-factor authentication operations
/// Supports TOTP (Time-based One-Time Password) and other 2FA methods
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Generates a new TOTP secret for authenticator app setup
    /// </summary>
    /// <returns>Base32-encoded secret</returns>
    string GenerateTotpSecret();

    /// <summary>
    /// Generates a QR code URL for authenticator app setup
    /// </summary>
    /// <param name="issuer">Application name</param>
    /// <param name="accountIdentifier">User identifier (email or username)</param>
    /// <param name="secret">TOTP secret</param>
    /// <returns>QR code URL for Google Authenticator format</returns>
    string GenerateQrCodeUrl(string issuer, string accountIdentifier, string secret);

    /// <summary>
    /// Verifies a TOTP code against a secret
    /// </summary>
    /// <param name="secret">TOTP secret</param>
    /// <param name="code">6-digit code from authenticator app</param>
    /// <returns>True if code is valid, false otherwise</returns>
    bool VerifyTotpCode(string secret, string code);

    /// <summary>
    /// Generates backup codes for account recovery
    /// </summary>
    /// <param name="count">Number of codes to generate (default 10)</param>
    /// <returns>List of backup codes</returns>
    List<string> GenerateBackupCodes(int count = 10);

    /// <summary>
    /// Hashes a backup code for secure storage
    /// </summary>
    /// <param name="code">Plain backup code</param>
    /// <returns>Hashed code</returns>
    string HashBackupCode(string code);

    /// <summary>
    /// Verifies a backup code
    /// </summary>
    /// <param name="code">Plain backup code</param>
    /// <param name="hashedCodes">List of hashed backup codes</param>
    /// <returns>True if code matches and is valid, false otherwise</returns>
    bool VerifyBackupCode(string code, List<string> hashedCodes);

    /// <summary>
    /// Generates a verification code for SMS or Email
    /// </summary>
    /// <returns>6-digit numeric code</returns>
    string GenerateVerificationCode();
}
