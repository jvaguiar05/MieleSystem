using FluentValidation.Results;

namespace MieleSystem.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando uma ou mais validações falham.
/// Utilizada em conjunto com o ValidationBehavior.
/// </summary>
public class ValidationException : ApplicationException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Uma ou mais validações falharam.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
