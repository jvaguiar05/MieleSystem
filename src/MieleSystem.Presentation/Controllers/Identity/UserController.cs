using MediatR;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Identity.Features.User.Queries.ListUsersPaged;

namespace MieleSystem.Presentation.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class UserController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
    : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get a paginated list of users.
    /// </summary>
    /// <returns>A paged list of users in the system.</returns>
    [HttpGet("users-paged")]
    public async Task<IActionResult> GetUsersPaged([FromQuery] ListUsersPagedQuery query)
    {
        var users = await _mediator.Send(query);
        return Ok(users);
    }
}
