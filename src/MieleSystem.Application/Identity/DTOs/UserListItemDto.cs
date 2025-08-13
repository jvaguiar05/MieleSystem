namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO com dados básicos para listagem de usuários.
/// </summary>
public sealed class UserListItemDto
{
    public Guid PublicId { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Role { get; init; } = null!;
}
