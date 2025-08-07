using MieleSystem.Application.Common.Interfaces;

namespace MieleSystem.Application.Common.Utils;

/// <summary>
/// Implementação padrão de IDateTimeProvider usando o tempo do sistema (UTC).
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
