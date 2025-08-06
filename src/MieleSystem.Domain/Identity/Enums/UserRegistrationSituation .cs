namespace MieleSystem.Domain.Identity.Enums;

/// <summary>
/// Representa o status da conta do usuário.
/// Define se o usuário está ativo, pendente de aprovação ou suspenso.
/// </summary>
public enum UserRegistrationSituation
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
}
