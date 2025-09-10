using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;
using MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;
using MieleSystem.Application.Identity.Features.User.Commands.VerifyLoginOtp;
using MieleSystem.Application.Identity.Services.Authentication;
using MieleSystem.Presentation.Extensions;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator, IHttpContextAuthenticationService authService)
    : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IHttpContextAuthenticationService _authService = authService;

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
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return result.ToActionResult();

        var loginData = result.Value;

        if (loginData == null || loginData.PlainTextRefreshToken == null)
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Dados de login inválidos."
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
}
