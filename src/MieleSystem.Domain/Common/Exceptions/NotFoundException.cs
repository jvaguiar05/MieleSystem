namespace MieleSystem.Domain.Common.Exceptions;

/// <summary>
/// Exceção lançada quando uma entidade ou recurso esperado não é encontrado.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} com identificador '{key}' não foi encontrado.") { }
}
