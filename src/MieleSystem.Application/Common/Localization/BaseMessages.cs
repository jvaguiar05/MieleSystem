namespace MieleSystem.Application.Common.Localization;

/// <summary>
/// Mensagens padrão globais, usadas por múltiplos contextos.
/// </summary>
public abstract class BaseMessages : IMessageProvider
{
    public virtual string UnexpectedError =>
        "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    public virtual string InvalidRequest => "Requisição inválida. Verifique os dados enviados.";

    public virtual string Unauthorized => "Você não tem permissão para realizar esta ação.";

    public virtual string OperationSuccess => "Operação realizada com sucesso.";
}
