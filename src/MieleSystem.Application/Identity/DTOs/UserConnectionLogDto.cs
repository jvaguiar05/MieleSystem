namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO para consulta de UserConnectionLog via ReadStore.
/// </summary>
public sealed class UserConnectionLogDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string IpAddress { get; init; } = null!;
    public string UserAgent { get; init; } = null!;
    public string? DeviceId { get; init; }
    public string? Location { get; init; }
    public DateTime ConnectedAtUtc { get; init; }
    public bool IsSuccessful { get; init; }
    public bool RequiredOtp { get; init; }
    public string? OtpReason { get; init; }
    public string? AdditionalInfo { get; init; }
    public int UserId { get; init; }
}
