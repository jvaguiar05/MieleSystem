using FluentValidation;
using MieleSystem.Application.Common.DTOs;

namespace MieleSystem.Application.Common.Validators;

/// <summary>
/// Validação padrão para requisições paginadas.
/// Garante que valores estejam dentro dos limites esperados.
/// </summary>
public class PageRequestValidator : AbstractValidator<PageRequestDto>
{
    public PageRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("A página deve ser maior que 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("O tamanho da página deve ser maior que 0.")
            .LessThanOrEqualTo(PageRequestDto.MaxPageSize)
            .WithMessage($"O tamanho máximo da página é {PageRequestDto.MaxPageSize}.");
    }
}
