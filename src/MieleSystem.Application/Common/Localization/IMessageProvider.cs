namespace MieleSystem.Application.Common.Localization;

/// <summary>
/// Interface que define um provedor de mensagens.
/// Pode ser usada para suporte a múltiplos idiomas no futuro.
/// </summary>
public interface IMessageProvider
{
    string UnexpectedError { get; }
    string InvalidRequest { get; }
    string Unauthorized { get; }
    string OperationSuccess { get; }
}
