using System.Globalization;
using MieleSystem.Application.Identity.Services.Email;

namespace MieleSystem.Infrastructure.Identity.Email;

/// <summary>
/// Responsável por gerar conteúdo HTML simples para e-mails transacionais do sistema.
/// </summary>
public sealed class SimpleEmailTemplateRenderer : IEmailTemplateRenderer
{
    public string RenderWelcome(string userName)
    {
        return $"""
                <html>
                    <body style="font-family: sans-serif; line-height: 1.5;">
                        <h2>Bem-vindo ao MieleSystem, {userName}!</h2>
                        <p>Estamos felizes em tê-lo(a) conosco.</p>
                        <p>Se tiver dúvidas ou precisar de ajuda, entre em contato com nosso suporte.</p>
                        <br/>
                        <p>Para começar, aguarde a aprovação do seu cadastro por um administrador.</p>
                        <p>Assim que aprovado, você poderá acessar sua conta e explorar todas as funcionalidades do MieleSystem.</p>
                        <p>Entraremos em contato assim que sua conta for ativada.</p>
                        <br/>
                        <p>Atenciosamente,</p>
                        <strong>Equipe MieleSystem</strong>
                    </body>
                </html>
            """;
    }

    public string RenderOtp(string code, DateTime expiresAtUtc)
    {
        var expiresLocal = TimeZoneInfo.ConvertTimeFromUtc(expiresAtUtc, TimeZoneInfo.Local);
        var expirationFormatted = expiresLocal.ToString(
            "dd/MM/yyyy HH:mm",
            CultureInfo.GetCultureInfo("pt-BR")
        );

        return $"""
                <html>
                    <body style="font-family: sans-serif; line-height: 1.5;">
                        <h2>Seu código de verificação</h2>
                        <p>Use o código abaixo para validar sua identidade no MieleSystem:</p>
                        <h1 style="letter-spacing: 3px;">{code}</h1>
                        <p><strong>Válido até:</strong> {expirationFormatted}</p>
                        <p>Se você não solicitou este código, ignore este e-mail.</p>
                        <br/>
                        <p>Equipe MieleSystem</p>
                    </body>
                </html>
            """;
    }

    public string RenderPasswordChanged(DateTime changedAtUtc)
    {
        var changedAtLocal = TimeZoneInfo.ConvertTimeFromUtc(changedAtUtc, TimeZoneInfo.Local);
        var formatted = changedAtLocal.ToString(
            "dd/MM/yyyy HH:mm",
            CultureInfo.GetCultureInfo("pt-BR")
        );

        return $"""
                <html>
                    <body style="font-family: sans-serif; line-height: 1.5;">
                        <h2>Sua senha foi alterada</h2>
                        <p>Informamos que sua senha foi alterada em:</p>
                        <p><strong>{formatted}</strong></p>
                        <p>Se essa ação não foi realizada por você, entre em contato com nosso suporte imediatamente.</p>
                        <br/>
                        <p>Equipe MieleSystem</p>
                    </body>
                </html>
            """;
    }

    public string RenderAccountActivated(string userName)
    {
        return $"""
                <html>
                    <body style="font-family: sans-serif; line-height: 1.5;">
                        <h2>Conta Ativada</h2>
                        <p>Olá {userName},</p>
                        <p>Sua conta no MieleSystem foi ativada com sucesso!</p>
                        <p>Agora você pode acessar sua conta e explorar todas as funcionalidades do sistema.</p>
                        <br/>
                        <p>Se tiver dúvidas ou precisar de ajuda, entre em contato com nosso suporte.</p>
                        <br/>
                        <p>Atenciosamente,</p>
                        <strong>Equipe MieleSystem</strong>
                    </body>
                </html>
            """;
    }
}
