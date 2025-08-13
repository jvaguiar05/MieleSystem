using MediatR;
using MieleSystem.Domain.Common.Interfaces;

namespace MieleSystem.Application.Common.Events;

/// <summary>
/// Representa um wrapper genérico que permite a publicação de eventos de domínio puros
/// via MediatR, sem acoplar o domínio à infraestrutura.
/// </summary>
/// <typeparam name="TEvent">Tipo do evento de domínio.</typeparam>
public sealed class EventNotification<TEvent>(TEvent domainEvent) : INotification
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Evento de domínio encapsulado.
    /// </summary>
    public TEvent DomainEvent { get; } = domainEvent;
}
