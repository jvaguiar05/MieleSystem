using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Domain.Common.Events;

/// <summary>
/// Evento de domínio base. Pode ser herdado por eventos específicos em cada contexto.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    // Identificador único do evento, gerado automaticamente
    public Guid EventId { get; } = Guid.NewGuid();

    // Momento em que o evento foi gerado, usando UTC para consistência
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
