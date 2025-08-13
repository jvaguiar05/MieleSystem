namespace MieleSystem.Application.Common.Localization.Messages;

/// <summary>
/// Contém mensagens padronizadas de erro e sucesso usadas em Responses.
/// Separado por categorias para facilitar leitura, tradução e consistência.
/// </summary>
public static class Messages
{
    // public static class UserMessages
    // {
    //     public const string NotFound = "Usuário não encontrado.";
    //     public const string EmailAlreadyInUse = "O e-mail informado já está em uso.";
    //     public const string InvalidPassword = "Senha incorreta.";
    //     public const string Registered = "Usuário registrado com sucesso.";
    //     public const string Updated = "Dados do usuário atualizados.";
    // }

    // public static class AuthMessages
    // {
    //     public const string InvalidToken = "O token de acesso é inválido ou expirou.";
    //     public const string OtpExpired = "O código OTP expirou. Solicite um novo.";
    //     public const string OtpInvalid = "O código OTP é inválido.";
    //     public const string LoginSuccess = "Login realizado com sucesso.";
    //     public const string LogoutSuccess = "Sessão encerrada com sucesso.";
    // }

    public static class GeneralMessages
    {
        public const string UnexpectedError =
            "Ocorreu um erro inesperado. Tente novamente mais tarde.";
        public const string InvalidRequest = "Requisição inválida. Verifique os dados enviados.";
        public const string Unauthorized = "Você não tem permissão para realizar esta ação.";
        public const string OperationSuccess = "Operação realizada com sucesso.";
    }
}
