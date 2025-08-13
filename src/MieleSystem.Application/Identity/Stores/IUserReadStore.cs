using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Stores;

/// <summary>
/// Fonte de leitura otimizada para listagem e consulta de usuários.
/// Retorna DTOs diretamente, sem carregar entidades de domínio.
/// </summary>
public interface IUserReadStore
{
    Task<PageResultDto<UserListItemDto>> GetPagedAsync(
        PageRequestDto request,
        CancellationToken ct = default
    );
}
