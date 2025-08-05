namespace MieleSystem.Domain.Common.Exceptions;

/// <summary>
/// Exceção genérica do domínio. Base para outras exceções.
/// </summary>
public class DomainException(string message) : Exception(message) { }
