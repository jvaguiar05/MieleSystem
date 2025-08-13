using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Services.Interfaces;

/// <summary>
/// Serviço de envio de e-mails relacionado a operações de conta no contexto de Identity.
///
/// 🔹 Definido na camada Application para seguir o princípio de inversão de dependências (DIP).
/// 🔹 Implementações concretas residem na camada Infrastructure.Identity.
/// 🔹 Consome o ValueObject <see cref="Email"/> para garantir validade e consistência de endereços.
///
/// Responsável por executar comunicações automáticas de eventos importantes da conta:
/// - Boas-vindas a novos usuários
/// - Envio de códigos OTP (autenticação de dois fatores)
/// - Notificação de alteração de senha
///
/// Observação:
/// Esta interface **não** deve conter lógica de formatação de templates.
/// A formatação deve ser responsabilidade da implementação concreta.
/// </summary>
public interface IAccountEmailService
{
    /// <summary>
    /// Envia um e-mail de boas-vindas para um novo usuário.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino, encapsulado no VO <see cref="Email"/>.</param>
    /// <param name="userName">Nome de exibição do usuário.</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task SendWelcomeAsync(Email to, string userName, CancellationToken ct = default);

    /// <summary>
    /// Envia um e-mail contendo código OTP para autenticação de dois fatores.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="code">Código OTP a ser enviado.</param>
    /// <param name="expiresAtUtc">Data e hora de expiração do código (em UTC).</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task SendOtpAsync(Email to, string code, DateTime expiresAtUtc, CancellationToken ct = default);

    /// <summary>
    /// Envia um e-mail notificando alteração de senha na conta.
    /// </summary>
    /// <param name="to">Endereço de e-mail de destino.</param>
    /// <param name="changedAtUtc">Data e hora da alteração (em UTC).</param>
    /// <param name="ct">Token de cancelamento opcional.</param>
    Task SendPasswordChangedAsync(Email to, DateTime changedAtUtc, CancellationToken ct = default);
}
