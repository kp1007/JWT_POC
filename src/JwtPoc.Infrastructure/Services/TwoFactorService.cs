using System.Security.Cryptography;
using OtpNet;
using JwtPoc.Core.Interfaces;

namespace JwtPoc.Infrastructure.Services;

/// <summary>
/// Two-factor authentication service
/// Implements TOTP (Time-based One-Time Password) for authenticator apps
/// Compatible with Google Authenticator, Authy, Microsoft Authenticator, etc.
/// </summary>
public class TwoFactorService : ITwoFactorService
{
    private readonly IPasswordHasher _passwordHasher;

    public TwoFactorService(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string GenerateTotpSecret()
    {
        // Generate a random 20-byte secret
        // This is the standard secret size for TOTP
        var secretBytes = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(secretBytes);
        }

        // Encode to Base32 (required format for authenticator apps)
        return Base32Encoding.ToString(secretBytes);
    }

    public string GenerateQrCodeUrl(string issuer, string accountIdentifier, string secret)
    {
        // Generate otpauth URL for QR code
        // This URL follows the Google Authenticator URL scheme
        // Format: otpauth://totp/Issuer:Account?secret=SECRET&issuer=Issuer
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedAccount = Uri.EscapeDataString(accountIdentifier);
        var encodedSecret = Uri.EscapeDataString(secret);

        return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={encodedSecret}&issuer={encodedIssuer}";
    }

    public bool VerifyTotpCode(string secret, string code)
    {
        try
        {
            // Decode the Base32 secret
            var secretBytes = Base32Encoding.ToBytes(secret);

            // Create TOTP generator with 30-second time step (standard)
            var totp = new Totp(secretBytes, step: 30, totpSize: 6);

            // Verify code with time window tolerance
            // Allows for 1 step before and after current time (90 seconds total window)
            // This accounts for time sync issues between server and client
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch
        {
            return false;
        }
    }

    public List<string> GenerateBackupCodes(int count = 10)
    {
        var codes = new List<string>();
        using var rng = RandomNumberGenerator.Create();

        for (int i = 0; i < count; i++)
        {
            // Generate 8-character alphanumeric code
            // Format: XXXX-XXXX for readability
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            var code = Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .ToUpper()
                .Substring(0, 8);

            // Format with hyphen for readability
            var formattedCode = $"{code.Substring(0, 4)}-{code.Substring(4, 4)}";
            codes.Add(formattedCode);
        }

        return codes;
    }

    public string HashBackupCode(string code)
    {
        // Use the same password hashing mechanism for backup codes
        // This ensures backup codes are never stored in plain text
        return _passwordHasher.HashPassword(code);
    }

    public bool VerifyBackupCode(string code, List<string> hashedCodes)
    {
        // Check if the provided code matches any of the hashed backup codes
        foreach (var hashedCode in hashedCodes)
        {
            if (_passwordHasher.VerifyPassword(code, hashedCode))
            {
                return true;
            }
        }

        return false;
    }

    public string GenerateVerificationCode()
    {
        // Generate a 6-digit numeric code for SMS/Email
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);

        // Convert to integer and get 6 digits
        var value = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return value.ToString("D6");
    }
}
