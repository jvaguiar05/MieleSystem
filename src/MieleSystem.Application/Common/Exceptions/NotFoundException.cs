namespace MieleSystem.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso solicitado não é encontrado.
/// Exemplo: busca por ID inexistente.
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base($"{name} com chave '{key}' não foi encontrado.") { }
}
