namespace MieleSystem.Application.Common.DTOs;

/// <summary>
/// Representa o resultado de uma consulta paginada.
/// </summary>
/// <typeparam name="T">Tipo de item contido na página.</typeparam>
public class PageResultDto<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    /// <summary>
    /// Total de páginas disponíveis com base no total de itens e page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica se há mais páginas além da atual.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica se há páginas anteriores à atual.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
