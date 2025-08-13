using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;

/// <summary>
/// Handler responsável por orquestrar o registro de um novo usuário.
/// Regras:
/// - Verificar unicidade de e-mail
/// - Hash da senha
/// - Definir Role (padrão: Viewer)
/// - Persistir agregado e confirmar UnitOfWork
/// - Retornar o identificador público do usuário criado
/// </summary>
public sealed class RegisterUserHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IUnitOfWork uow
) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository _users = users;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        // 1) Normaliza e valida VOs básicos
        var emailVo = new Email(request.Email);

        // 2) Garantir unicidade de e-mail
        var exists = await _users.ExistsByEmailAsync(emailVo, ct);
        if (exists)
            return Result<Guid>.Failure("O e-mail informado já está em uso.");

        // 3) Gerar hash seguro da senha
        var hashStr = _passwordHasher.Hash(request.Password);
        var passwordVo = new PasswordHash(hashStr);

        // 4) Definir Role (padrão Viewer)
        var role = ResolveRoleOrDefault(request.Role);

        // 5) Criar agregado e adicionar ao repositório
        var user = Domain.Identity.Entities.User.Register(
            name: request.Name,
            email: emailVo,
            passwordHash: passwordVo,
            role: role
        );

        await _users.AddAsync(user, ct);

        // 6) Confirmar transação
        await _uow.SaveChangesAsync(ct);

        // 7) Retornar apenas o PublicId
        return Result<Guid>.Success(user.PublicId);
    }

    /// <summary>
    /// Resolve a Role a partir de string. Caso nula/inválida, retorna a padrão (editor).
    /// </summary>
    private static UserRole ResolveRoleOrDefault(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return UserRole.Editor;

        return
            Enum.TryParse(typeof(UserRole), roleName, true, out var parsed)
            && parsed is UserRole userRole
            ? userRole
            : UserRole.Editor;
    }
}
