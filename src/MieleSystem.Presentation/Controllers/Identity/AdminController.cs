using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Identity.Features.User.Commands.ApproveUserRegistration;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AdminController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
    : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Approves a user registration.
    /// </summary>
    /// <param name="command">The approval command containing user ID.</param>
    /// <returns>Success result if the user registration was approved.</returns>
    [AllowAnonymous]
    [HttpPost("approve-user-registration")]
    public async Task<IActionResult> ApproveUserRegistration(
        [FromBody] ApproveUserRegistrationCommand command
    )
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(result);

        return BadRequest(result);
    }
}
