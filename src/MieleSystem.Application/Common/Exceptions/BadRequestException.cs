namespace MieleSystem.Application.Common.Exceptions;

public class BadRequestException : ApplicationException
{
    public BadRequestException()
        : base(
            "A solicitação não pôde ser processada devido a um erro de sintaxe ou dados inválidos."
        ) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}
