using MieleSystem.Domain.Common.Base;

namespace MieleSystem.Domain.Identity.Enums;

/// <summary>
/// Representa os papéis possíveis de um usuário no sistema.
/// Utiliza o padrão Smart Enum (Enumeration).
/// </summary>
public sealed class UserRole : Enumeration
{
    public static readonly UserRole Admin = new(1, "Admin");
    public static readonly UserRole Editor = new(2, "Editor");
    public static readonly UserRole Viewer = new(3, "Viewer");
    public static readonly UserRole Suspended = new(4, "Suspended");

    private UserRole(int value, string name)
        : base(value, name) { }

    public static UserRole FromValue(int value)
    {
        return GetAll<UserRole>().FirstOrDefault(r => r.Value == value)
            ?? throw new ArgumentException($"Valor inválido para UserRole: {value}");
    }

    public static UserRole FromName(string name)
    {
        return GetAll<UserRole>()
                .FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Nome inválido para UserRole: {name}");
    }
}
