using System.Net.Mail;

namespace MieleSystem.Application.Common.Extensions;

/// <summary>
/// Métodos auxiliares para validação e normalização de e-mails.
/// </summary>
public static class EmailExtensions
{
    public static string NormalizeEmail(this string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public static bool IsValidEmailFormat(this string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    public static string? GetEmailDomain(this string email)
    {
        return email.Contains('@') ? email.Split('@').LastOrDefault() : null;
    }
}
