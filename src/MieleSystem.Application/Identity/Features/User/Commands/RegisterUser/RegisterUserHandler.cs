using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;

/// <summary>
/// Handler responsável por registrar um novo usuário no sistema.
/// Valida e-mail, aplica hash na senha, define o papel (Role),
/// persiste o agregado e dispara o evento de registro.
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
        // Normaliza e valida VOs básicos
        var emailVo = new Email(request.Email);

        // Garante unicidade de e-mail
        var exists = await _users.ExistsByEmailAsync(emailVo, ct);
        if (exists)
            return Result<Guid>.Failure(
                Error.Conflict("email.already_exists", "O e-mail informado já está em uso.")
            );

        // Gera hash seguro da senha
        var hashStr = _passwordHasher.Hash(request.Password);
        var passwordVo = new PasswordHash(hashStr);

        // Define Role (padrão Editor)
        var role = ResolveRoleOrDefault(request.Role);

        // Cria agregado e adiciona ao repositório
        var user = Domain.Identity.Entities.User.Register(
            name: request.Name,
            email: emailVo,
            passwordHash: passwordVo,
            role: role
        );

        await _users.AddAsync(user, ct);

        // Persiste para garantir que o Id seja gerado
        await _uow.SaveChangesAsync(ct);

        // Lança UserRegisteredEvent com int UserId real
        user.MarkAsRegistered();

        // Dispara eventos via UoW (publica o UserRegisteredEvent)
        await _uow.SaveChangesAsync(ct);

        // Retorna apenas o PublicId
        return Result<Guid>.Success(user.PublicId);
    }

    /// <summary>
    /// Resolve a Role a partir de string. Caso nula/inválida, retorna a padrão (editor).
    /// </summary>
    private static UserRole ResolveRoleOrDefault(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return UserRole.Editor;
        else if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException(
                "Não é permitido registrar usuários com o cargo 'Admin'."
            );

        return
            Enum.TryParse(typeof(UserRole), roleName, true, out var parsed)
            && parsed is UserRole userRole
            ? userRole
            : UserRole.Editor;
    }
}
