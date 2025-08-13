using MediatR;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;

/// <summary>
/// Comando para registrar um novo usuário no sistema.
/// </summary>
public sealed class RegisterUserCommand : IRequest<Result<Guid>>
{
    /// <summary>Nome completo do usuário.</summary>
    public string Name { get; init; } = null!;

    /// <summary>E-mail do usuário.</summary>
    public string Email { get; init; } = null!;

    /// <summary>Senha em texto claro (hasheada pela Infrastructure).</summary>
    public string Password { get; init; } = null!;

    /// <summary>
    /// Role opcional. Se não informado, aplica padrão (ex.: Viewer).
    /// </summary>
    public string? Role { get; init; }
}
