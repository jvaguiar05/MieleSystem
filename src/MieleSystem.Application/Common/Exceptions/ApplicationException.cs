namespace MieleSystem.Application.Common.Exceptions;

/// <summary>
/// Exceção base para todas as exceções específicas da camada de Application.
/// Permite tratamento uniforme no middleware da camada Presentation.
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message)
        : base(message) { }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException) { }
}
