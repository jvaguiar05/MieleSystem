using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;
using MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;
using MieleSystem.Domain.Identity.Services;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IHttpContextAccessor httpContextAccessor,
    IMediator mediator,
    ITokenService tokenService
) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ITokenService _tokenService = tokenService;

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <returns>Public ID (Guid) of the registered user.</returns>
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
    /// <returns>Access token (JWT) no corpo da resposta e o Refresh Token em um cookie HttpOnly.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            // Retorna 401 Unauthorized com a mensagem de erro fornecida pelo Handler
            return Unauthorized(result.Error);

        // Se o resultado for sucesso, extrai os valores
        var loginData = result.Value;

        if (loginData == null || loginData.PlainTextRefreshToken == null)
            // Retorna 500 Internal Server Error se os dados de login estiverem ausentes
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Dados de login inválidos."
            );

        // Configura o cookie seguro para o Refresh Token
        Response.Cookies.Append(
            "refreshToken",
            loginData.PlainTextRefreshToken,
            new CookieOptions
            {
                HttpOnly = true, // Impede o acesso do JavaScript, protegendo contra XSS
                Secure = true, // Garante que o cookie seja enviado apenas sobre HTTPS
                SameSite = SameSiteMode.Strict, // Melhor proteção contra ataques CSRF
                Expires =
                    loginData.RefreshTokenExpiresAtUtc // Define a expiração do cookie
            }
        );

        // Retorna 200 OK com o corpo do JSON contendo apenas o Access Token
        return Ok(loginData.Dto);
    }
}
