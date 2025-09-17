namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO detalhado com informações completas do usuário.
/// Usado para operações de consulta por ID.
/// </summary>
public sealed class UserDto
{
    public Guid PublicId { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Role { get; init; } = null!;
    public string RegistrationSituation { get; init; } = null!;
    public DateTime CreatedAtUtc { get; init; }
    public DateOnly? ExpiresAt { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
}
