namespace MieleSystem.Domain.Common.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// </summary>
public class BusinessRuleValidationException(string message) : DomainException(message) { }
