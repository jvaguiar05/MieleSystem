namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa os parâmetros de paginação utilizados em consultas (queries paginadas).
/// </summary>
public class PageRequestDto
{
    public const int MaxPageSize = 100;

    private int _page = 1;
    private int _pageSize = 20;

    /// <summary>
    /// Página solicitada (mínimo = 1).
    /// </summary>
    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Tamanho da página (máximo = 100).
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Campo de ordenação (ex: "name", "createdAt desc").
    /// </summary>
    public string? OrderBy { get; init; } = string.Empty;

    /// <summary>
    /// Quantidade de itens a pular (offset).
    /// </summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>
    /// Quantidade de itens a retornar (limit).
    /// </summary>
    public int Take => PageSize;
}
