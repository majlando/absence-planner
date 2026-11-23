using Microsoft.AspNetCore.Identity;

namespace Business;

// Hashes passwords with ASP.NET Core's PasswordHasher (PBKDF2 with a random salt).
public static class Passwords
{
    private static readonly PasswordHasher<object> Hasher = new();

    public static string Hash(string password) => Hasher.HashPassword(null!, password);

    public static bool Verify(string hash, string password) =>
        Hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
