using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Identity.DTOs;

namespace MieleSystem.Application.Identity.Stores;

/// <summary>
/// Fonte de leitura otimizada para listagem e consulta de usuários.
/// Retorna DTOs diretamente, sem carregar entidades de domínio.
/// </summary>
public interface IUserReadStore
{
    /// <summary>
    /// Obtém uma página de usuários com base nos critérios de paginação fornecidos.
    /// </summary>
    /// <param name="request">Critérios de paginação</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Uma página de usuários</returns>
    Task<PageResultDto<UserListItemDto>> GetPagedAsync(
        PageRequestDto request,
        CancellationToken ct = default
    );

    /// <summary>
    /// Obtém um usuário pelo seu ID público.
    /// </summary>
    /// <param name="publicId">ID público do usuário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados detalhados do usuário ou null se não encontrado</returns>
    Task<UserDto?> GetByPublicIdAsync(Guid publicId, CancellationToken ct = default);
}
