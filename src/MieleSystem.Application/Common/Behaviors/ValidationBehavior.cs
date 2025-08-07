using FluentValidation;
using MediatR;
using Exception = MieleSystem.Application.Common.Exceptions.ValidationException;

namespace MieleSystem.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior que executa as validações do FluentValidation
/// antes de passar o controle para o handler do comando/query.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default
    )
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count > 0)
                throw new Exception(failures);
        }

        return await next(ct);
    }
}
