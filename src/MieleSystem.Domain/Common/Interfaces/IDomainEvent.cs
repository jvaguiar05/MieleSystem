namespace MieleSystem.Domain.Common.Interfaces;

/// <summary>
/// Interface para eventos de domínio.
/// Representa ações significativas que ocorreram no sistema.
/// Pode ser usada para rastreamento, auditoria ou processamento assíncrono.
/// </summary>
public interface IDomainEvent
{
    /// Identificador único do evento.
    Guid EventId { get; }

    /// Momento em que o evento foi gerado.
    DateTime OccurredOn { get; }
}
