using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;
using MieleSystem.Application.Identity.Features.User.Commands.LogoutUser;
using MieleSystem.Application.Identity.Features.User.Commands.RefreshToken;
using MieleSystem.Application.Identity.Features.User.Commands.RegenerateOtp;
using MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;
using MieleSystem.Application.Identity.Features.User.Commands.VerifyLoginOtp;
using MieleSystem.Application.Identity.Services.Authentication;
using MieleSystem.Presentation.Extensions;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IMediator mediator,
    IHttpContextAuthenticationService authService,
    ILogger<AuthController> logger
) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IHttpContextAuthenticationService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    /// <param name="command">Comando de registro contendo nome, email, senha e cargo (opcional).</param>
    /// <returns>Public ID (Guid) do usuário registrado.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Autentica um usuário existente.
    /// </summary>
    /// <param name="command">Comando de login contendo email e senha.</param>
    /// <returns>Access token (JWT) no corpo da resposta e o Refresh Token em um cookie HttpOnly.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var clientInfo = _authService.GetCurrentClientInfo();

        var commandWithClientInfo = new LoginUserCommand
        {
            Email = command.Email,
            Password = command.Password,
            ClientIp = clientInfo?.IpAddress,
            UserAgent = clientInfo?.UserAgent,
            DeviceId = clientInfo?.DeviceId,
        };

        var result = await _mediator.Send(commandWithClientInfo);

        if (!result.IsSuccess)
            return result.ToActionResult();

        var loginData = result.Value;

        if (loginData == null || loginData.PlainTextRefreshToken == null)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Dados de login inválidos."
            );

        _logger.LogInformation(
            "Appending refresh token {RefreshToken} to response cookies for user {Email}",
            loginData.PlainTextRefreshToken,
            command.Email
        );

        Response.Cookies.Append(
            "refreshToken",
            loginData.PlainTextRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginData.RefreshTokenExpiresAtUtc,
            }
        );

        return Ok(loginData.Dto);
    }

    /// <summary>
    /// Verifica o código OTP e completa o processo de autenticação.
    /// </summary>
    /// <param name="command">Comando de verificação OTP contendo email e código OTP.</param>
    /// <returns>Access token (JWT) no corpo da resposta e o Refresh Token em um cookie HttpOnly.</returns>
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyLoginOtpCommand command)
    {
        var clientInfo = _authService.GetCurrentClientInfo();

        var commandWithClientInfo = new VerifyLoginOtpCommand
        {
            Email = command.Email,
            OtpCode = command.OtpCode,
            ClientIp = clientInfo?.IpAddress,
            UserAgent = clientInfo?.UserAgent,
            DeviceId = clientInfo?.DeviceId,
        };

        var result = await _mediator.Send(commandWithClientInfo);

        if (!result.IsSuccess)
            return result.ToActionResult();

        var loginData = result.Value;

        if (loginData == null || loginData.PlainTextRefreshToken == null)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Dados de login inválidos."
            );

        _logger.LogInformation(
            "Appending refresh token {RefreshToken} to response cookies for user {Email}",
            loginData.PlainTextRefreshToken,
            command.Email
        );

        Response.Cookies.Append(
            "refreshToken",
            loginData.PlainTextRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginData.RefreshTokenExpiresAtUtc,
            }
        );

        return Ok(loginData.Dto);
    }

    /// <summary>
    /// Regenera um código OTP expirado para o usuário.
    /// </summary>
    /// <param name="command">Comando contendo o email do usuário.</param>
    /// <returns>Resultado da regeneração de OTP.</returns>
    [AllowAnonymous]
    [HttpPost("regenerate-otp")]
    public async Task<IActionResult> RegenerateOtp([FromBody] RegenerateOtpCommand command)
    {
        var clientInfo = _authService.GetCurrentClientInfo();

        var commandWithClientInfo = new RegenerateOtpCommand
        {
            Email = command.Email,
            ClientIp = clientInfo?.IpAddress,
            UserAgent = clientInfo?.UserAgent,
            DeviceId = clientInfo?.DeviceId,
        };

        var result = await _mediator.Send(commandWithClientInfo);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Renova o token de acesso usando um refresh token válido.
    /// </summary>
    /// <returns>Novo access token (JWT) no corpo da resposta e novo Refresh Token em um cookie HttpOnly.</returns>
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        // Obtém o refresh token do cookie HttpOnly
        if (
            !Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken)
        )
            return Unauthorized("Refresh token não encontrado ou inválido.");

        var clientInfo = _authService.GetCurrentClientInfo();

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshToken,
            ClientIp = clientInfo?.IpAddress,
            UserAgent = clientInfo?.UserAgent,
            DeviceId = clientInfo?.DeviceId,
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return result.ToActionResult();

        var loginData = result.Value;

        if (loginData == null || loginData.PlainTextRefreshToken == null)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Dados de renovação de token inválidos."
            );

        _logger.LogInformation(
            "Appending new refresh token {RefreshToken} to response cookies after token refresh",
            loginData.PlainTextRefreshToken
        );

        // Define o novo refresh token no cookie HttpOnly
        Response.Cookies.Append(
            "refreshToken",
            loginData.PlainTextRefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginData.RefreshTokenExpiresAtUtc,
            }
        );

        return Ok(loginData.Dto);
    }

    /// <summary>
    /// Realiza logout do usuário revogando o refresh token e limpando cookies.
    /// </summary>
    /// <returns>Confirmação do logout.</returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var currentUserId = User.GetUserId();
        var clientInfo = _authService.GetCurrentClientInfo();

        // Obtém o refresh token do cookie HttpOnly (pode ser null)
        Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

        var command = new LogoutUserCommand
        {
            RefreshToken = refreshToken,
            CurrentUserPublicId = currentUserId,
            ClientIp = clientInfo?.IpAddress,
            UserAgent = clientInfo?.UserAgent,
            DeviceId = clientInfo?.DeviceId,
        };

        var result = await _mediator.Send(command);

        // Remove o cookie do refresh token independentemente do resultado
        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            }
        );

        if (!result.IsSuccess)
            return result.ToActionResult();

        return Ok(new { Message = "Logout realizado com sucesso." });
    }
}
