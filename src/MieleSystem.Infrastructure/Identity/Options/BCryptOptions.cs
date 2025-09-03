namespace MieleSystem.Infrastructure.Identity.Options;

/// <summary>
/// Opções para configuração do BCrypt.
/// </summary>
public sealed class BCryptOptions
{
    public int WorkFactor { get; init; } = default!;
}
