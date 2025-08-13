using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Domain.Common.Base;

/// <summary>
/// Aggregate Root — representa o ponto de entrada e consistência de um conjunto de entidades relacionadas.
/// Permite acionar eventos de domínio quando algo relevante acontece.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <inheritdoc cref="Entity"/>
    protected AggregateRoot(Guid publicId)
        : base(publicId) { }

    /// <summary>
    /// Construtor sem parâmetros protegido para uso do EF Core e serializadores.
    /// </summary>
    protected AggregateRoot()
        : base(Guid.Empty) { }

    // Lista privada de eventos de domínio
    private readonly List<IDomainEvent> _domainEvents = new();

    // Exposição pública somente leitura dos eventos
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Adiciona um evento de domínio à lista
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Alias opcional para semântica mais clara
    protected void Raise(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);

    // Remove todos os eventos registrados
    public void ClearDomainEvents() => _domainEvents.Clear();
}
