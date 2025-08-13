namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa o resultado de uma consulta paginada.
/// </summary>
/// <typeparam name="T">Tipo de item contido na página.</typeparam>
public sealed class PageResultDto<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    public int Page { get; init; } = 1;

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    /// <summary>
    /// Total de páginas disponíveis com base no total de itens e page size.
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
    /// Cria um PageResultDto com todos os campos calculados automaticamente.
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
            PageSize = pageSize <= 0 ? 1 : pageSize,
            TotalCount = totalCount < 0 ? 0 : totalCount,
        };
    }
}
