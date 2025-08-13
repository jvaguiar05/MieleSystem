namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa o resultado de uma consulta paginada.
/// </summary>
/// <typeparam name="T">Tipo de item contido na página.</typeparam>
public sealed class PageResultDto<T>
{
    /// <summary>
    /// Itens retornados na página atual.
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    /// <summary>
    /// Página atual (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Tamanho da página (quantidade de itens por página).
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Quantidade total de itens da fonte de dados.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Quantidade total de páginas disponíveis.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica se há mais páginas além da atual.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica se há páginas anteriores à atual.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Cria um PageResultDto com validação básica.
    /// Corrige valores inválidos de página, tamanho ou total.
    /// </summary>
    public static PageResultDto<T> Create(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount
    )
    {
        return new PageResultDto<T>
        {
            Items = items ?? Enumerable.Empty<T>(),
            Page = page < 1 ? 1 : page,
            PageSize = pageSize <= 0 ? 10 : pageSize,
            TotalCount = totalCount < 0 ? 0 : totalCount,
        };
    }
}
