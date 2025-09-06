namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO para consulta de RefreshToken via ReadStore.
/// </summary>
public sealed class RefreshTokenDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string TokenHash { get; init; } = null!;
    public DateTime ExpiresAtUtc { get; init; }
    public bool IsRevoked { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public int UserId { get; init; }
}
