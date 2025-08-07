namespace MieleSystem.Application.Common.Interfaces;

/// <summary>
/// Interface para acessar informações do usuário autenticado atual.
/// A implementação será feita na Infrastructure com base no HttpContext.
/// </summary>
public interface ICurrentUserAccessor
{
    Guid? GetUserId();
    string? GetUserEmail();
    bool IsAuthenticated();
}
