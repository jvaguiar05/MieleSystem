using MediatR;
using MieleSystem.Application.Common.Extensions;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Application.Identity.Features.User.Commands.LoginUser;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.RefreshToken;

/// <summary>
/// Handler responsável por renovar o token de acesso usando um refresh token válido.
/// </summary>
public sealed class RefreshTokenHandler(
    IUserRepository users,
    IRefreshTokenReadStore refreshTokenReadStore,
    ITokenService tokenService,
    IRefreshTokenHasher refreshTokenHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<RefreshTokenCommand, Result<LoginUserResult>>
{
    private readonly IUserRepository _users = users;
    private readonly IRefreshTokenReadStore _refreshTokenReadStore = refreshTokenReadStore;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IRefreshTokenHasher _refreshTokenHasher = refreshTokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LoginUserResult>> Handle(
        RefreshTokenCommand request,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<LoginUserResult>.Failure(
                Error.Unauthorized("Refresh token é obrigatório.")
            );
        }

        // Hash do refresh token fornecido para busca
        var refreshTokenHash = _refreshTokenHasher.Hash(request.RefreshToken);
        var refreshTokenHashVo = new RefreshTokenHash(refreshTokenHash);

        // Busca o refresh token no store de leitura
        var refreshTokenDto = await _refreshTokenReadStore.GetByTokenHashAsync(
            refreshTokenHashVo,
            ct
        );

        if (refreshTokenDto == null)
        {
            return Result<LoginUserResult>.Failure(
                Error.Unauthorized("Refresh token inválido ou expirado.")
            );
        }

        // Verifica se o token está válido (não revogado e não expirado)
        if (refreshTokenDto.IsRevoked || refreshTokenDto.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result<LoginUserResult>.Failure(
                Error.Unauthorized("Refresh token inválido ou expirado.")
            );
        }

        // Busca o usuário dono do refresh token
        var user = await _users.GetByIdAsync(refreshTokenDto.UserId, ct);
        if (user == null)
        {
            return Result<LoginUserResult>.Failure(Error.Unauthorized("Usuário não encontrado."));
        }

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            // Revoga o refresh token atual
            user.RevokeRefreshToken(refreshTokenHashVo);

            // Remove refresh tokens inválidos para limpeza
            user.RemoveInvalidRefreshTokens();

            // Gera novos tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var newPlainTextRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _refreshTokenHasher.Hash(newPlainTextRefreshToken);
            var newRefreshTokenHashVo = new RefreshTokenHash(newRefreshTokenHash);

            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();
            user.AddRefreshToken(newRefreshTokenHashVo, refreshTokenExpiresAt);

            // Log da conexão bem-sucedida
            var successConnectionLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "Token renovado com sucesso"
            );
            successConnectionLog.MarkAsSuccessful();

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            var resultDto = new LoginResultDto
            {
                AccessToken = accessToken,
                ExpiresAt = _tokenService.GetAccessTokenExpiration(),
            };

            var finalResult = new LoginUserResult(
                resultDto,
                newPlainTextRefreshToken,
                refreshTokenExpiresAt
            );

            return Result<LoginUserResult>.Success(finalResult);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);

            // Log do erro de infraestrutura
            var infrastructureFailureLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: $"Renovação de token falhou: erro de infraestrutura - {ex.Message}"
            );

            try
            {
                _users.Update(user);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch { }

            return Result<LoginUserResult>.Failure(
                Error.Infrastructure(
                    "refresh_token.infrastructure_error",
                    "Ocorreu um erro inesperado durante a renovação do token.",
                    details: ex.CreateExceptionDetails("RefreshToken")
                )
            );
        }
    }
}
