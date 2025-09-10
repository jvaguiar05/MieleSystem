using MieleSystem.Domain.Identity.Enums;

namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO para consulta de OtpSession via ReadStore.
/// </summary>
public sealed class OtpSessionDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string Code { get; init; } = null!;
    public DateTime ExpiresAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public bool IsUsed { get; init; }
    public OtpPurpose Purpose { get; init; }
    public DateTime? UsedAtUtc { get; init; }
    public int UserId { get; init; }
}
