namespace MieleSystem.Application.Common.Utils;

/// <summary>
/// Utilitário para geração de credenciais seguras como senhas e nomes de usuário.
/// </summary>
public static class SecurityGenerator
{
    private static readonly char[] PasswordChars =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789@#$%!".ToCharArray();

    private static readonly char[] UsernameChars =
        "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

    public static string GeneratePassword(int length = 12)
    {
        var rng = new Random();
        return new string(
            [
                .. Enumerable
                    .Range(0, length)
                    .Select(_ => PasswordChars[rng.Next(PasswordChars.Length)]),
            ]
        );
    }

    public static string GenerateUsername(string baseName, int length = 6)
    {
        var normalized = baseName
            .Trim()
            .ToLower()
            .Replace(" ", ".")
            .Replace("@", "")
            .Replace("-", "")
            .Replace("_", "");

        var suffix = Guid.NewGuid().ToString("N")[..length];
        return $"{normalized}.{suffix}";
    }
}
