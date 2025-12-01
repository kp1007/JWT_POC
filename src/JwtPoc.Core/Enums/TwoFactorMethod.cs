namespace JwtPoc.Core.Enums;

/// <summary>
/// Enumeration of supported two-factor authentication methods
/// </summary>
public enum TwoFactorMethod
{
    /// <summary>
    /// No 2FA enabled
    /// </summary>
    None = 0,

    /// <summary>
    /// Authenticator app using TOTP (e.g., Google Authenticator, Authy)
    /// Most secure option
    /// </summary>
    Authenticator = 1,

    /// <summary>
    /// SMS-based verification code
    /// Requires verified phone number
    /// </summary>
    Sms = 2,

    /// <summary>
    /// Email-based verification code
    /// Requires verified email address
    /// </summary>
    Email = 3
}
