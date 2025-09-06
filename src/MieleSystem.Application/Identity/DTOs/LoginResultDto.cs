namespace MieleSystem.Application.Identity.DTOs;

/// <summary>
/// DTO que representa o resultado de um login bem-sucedido.
/// </summary>
public sealed class LoginResultDto
{
    public string AccessToken { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
}
