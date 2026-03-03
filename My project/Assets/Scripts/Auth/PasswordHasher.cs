using System;
using System.Security.Cryptography;

public static class PasswordHasher
{
    private const int SaltSize = 16;      // 128 bits
    private const int HashSize = 32;      // 256 bits
    private const int Iterations = 120000;

    public static string GenerateSaltBase64()
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    public static string HashPassword(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);

        using (var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            return Convert.ToBase64String(hash);
        }
    }

    public static bool VerifyPassword(string password, string saltBase64, string expectedHashBase64)
    {
        string actualHashBase64 = HashPassword(password, saltBase64);

        byte[] a = Convert.FromBase64String(actualHashBase64);
        byte[] b = Convert.FromBase64String(expectedHashBase64);

        // Comparación en tiempo constante
        if (a.Length != b.Length) return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];

        return diff == 0;
    }
}
