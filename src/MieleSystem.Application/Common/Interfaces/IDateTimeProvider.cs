namespace MieleSystem.Application.Common.Interfaces;

/// <summary>
/// Abstração para obtenção de tempo atual (útil para testes e consistência).
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
