using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Aggregate Root — representa o ponto de entrada e consistência de um conjunto de entidades relacionadas.
/// Permite acionar eventos de domínio quando algo relevante acontece.
/// </summary>
public abstract class AggregateRoot(Guid publicId) : Entity(publicId)
{
    // Lista privada de eventos de domínio
    private readonly List<IDomainEvent> _domainEvents = new();

    // Exposição pública somente leitura dos eventos
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Adiciona um evento de domínio à lista
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Remove todos os eventos registrados
    public void ClearDomainEvents() => _domainEvents.Clear();
}
