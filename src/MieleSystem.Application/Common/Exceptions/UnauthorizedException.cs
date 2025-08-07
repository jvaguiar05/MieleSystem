namespace MieleSystem.Application.Common.Exceptions;

/// <summary>
/// Exceção lançada quando o usuário não está autorizado a realizar a operação solicitada.
/// Pode indicar falha de autenticação ou de permissão interna.
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException()
        : base("Você não está autorizado a realizar esta operação.") { }

    public UnauthorizedException(string message)
        : base(message) { }
}
