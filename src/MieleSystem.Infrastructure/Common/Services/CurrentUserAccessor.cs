using Microsoft.AspNetCore.Http;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Interfaces;

namespace MieleSystem.Infrastructure.Common.Services;

/// <summary>
/// Acesso ao usuário autenticado atual (via HttpContext/Claims).
/// Implementa ICurrentUserAccessor da camada Application.
/// </summary>
internal sealed class CurrentUserAccessor(IHttpContextAccessor http) : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _http = http;

    public Guid? GetUserId() => _http.HttpContext?.User.GetUserId();

    public string? GetUserEmail() => _http.HttpContext?.User.GetEmail();

    public bool IsAuthenticated() => _http.HttpContext?.User.IsAuthenticated() ?? false;
}
