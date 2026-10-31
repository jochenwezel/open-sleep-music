using System.Security.Cryptography;

namespace OpenSleepMusic.Core.Privacy;

/// <summary>A local parental gate, not an identity or age verification service.</summary>
public static class ParentPin
{
    private const int Iterations = 100_000;

    public static bool IsValid(string pin) => pin.Length is >= 6 and <= 12
        && pin.All(character => character is >= '0' and <= '9');

    public static string Create(string pin)
    {
        if (!IsValid(pin)) throw new ArgumentException("Use 6 to 12 digits.", nameof(pin));
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(pin, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"1.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string pin, string? credential)
    {
        if (!IsValid(pin) || credential is null) return false;
        var parts = credential.Split('.');
        if (parts.Length != 3 || parts[0] != "1") return false;
        try
        {
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);
            if (salt.Length != 16 || expected.Length != 32) return false;
            var actual = Rfc2898DeriveBytes.Pbkdf2(pin, salt, Iterations, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
