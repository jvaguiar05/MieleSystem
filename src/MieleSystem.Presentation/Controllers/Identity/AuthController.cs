using MediatR;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;
using MieleSystem.Application.Identity.Features.User.Commands.RegisterUser;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
    : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <returns>Public ID (Guid) of the registered user.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Login an existing user.
    /// </summary>
    /// <returns>Access token (JWT) for the logged-in user.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
