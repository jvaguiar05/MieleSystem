using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Identity.Features.User.Commands.DeleteUserProfile;
using MieleSystem.Application.Identity.Features.User.Queries.GetUserById;
using MieleSystem.Application.Identity.Features.User.Queries.ListUsersPaged;
using MieleSystem.Presentation.Extensions;

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

    /// <summary>
    /// Obtém um usuário pelo seu ID público.
    /// </summary>
    /// <param name="id">ID público do usuário (GUID)</param>
    /// <returns>Dados detalhados do usuário ou 404 se não encontrado</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery { PublicId = id };
        var user = await _mediator.Send(query);

        if (user == null)
            return NotFound($"Usuário com ID {id} não foi encontrado.");

        return Ok(user);
    }

    /// <summary>
    /// Permite que um usuário autenticado delete seu próprio perfil (soft delete).
    /// </summary>
    /// <returns>Confirmação da exclusão do perfil</returns>
    [Authorize]
    [HttpDelete("delete-profile")]
    public async Task<IActionResult> DeleteProfile()
    {
        var currentUserId = User.GetUserId();
        if (currentUserId == null)
            return Unauthorized("Usuário não autenticado.");

        var command = new DeleteUserProfileCommand { CurrentUserPublicId = currentUserId.Value };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return Ok(new { Message = "Perfil deletado com sucesso." });
    }
}
