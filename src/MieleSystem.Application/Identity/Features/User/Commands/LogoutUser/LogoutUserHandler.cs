using MediatR;
using MieleSystem.Application.Common.Responses;
using MieleSystem.Application.Identity.Stores;
using MieleSystem.Domain.Common.Interfaces;
using MieleSystem.Domain.Identity.Repositories;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Application.Identity.Features.User.Commands.LogoutUser;

/// <summary>
/// Handler responsável por processar o logout do usuário.
/// </summary>
public sealed class LogoutUserHandler(
    IUserRepository userRepository,
    IRefreshTokenReadStore refreshTokenReadStore,
    IRefreshTokenHasher refreshTokenHasher,
    IUnitOfWork unitOfWork
) : IRequestHandler<LogoutUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenReadStore _refreshTokenReadStore = refreshTokenReadStore;
    private readonly IRefreshTokenHasher _refreshTokenHasher = refreshTokenHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(
        LogoutUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Domain.Identity.Entities.User? user = null;

        // Tenta encontrar o usuário pelo refresh token primeiro
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var refreshTokenHash = _refreshTokenHasher.Hash(request.RefreshToken);
            var refreshTokenHashVo = new RefreshTokenHash(refreshTokenHash);

            var refreshTokenDto = await _refreshTokenReadStore.GetByTokenHashAsync(
                refreshTokenHashVo,
                cancellationToken
            );

            if (refreshTokenDto != null)
            {
                user = await _userRepository.GetByIdAsync(
                    refreshTokenDto.UserId,
                    cancellationToken
                );
            }
        }

        // Fallback: busca pelo ID do usuário autenticado se não encontrou pelo refresh token
        if (user == null && request.CurrentUserPublicId.HasValue)
        {
            user = await _userRepository.GetByPublicIdAsync(
                request.CurrentUserPublicId.Value,
                cancellationToken
            );
        }

        if (user == null)
        {
            // Não retorna erro para evitar enumeration attacks
            // Logout sempre retorna sucesso mesmo se o usuário não for encontrado
            return Result<bool>.Success(true);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Log de conexão para logout
            var logoutLog = user.AddConnectionLog(
                request.ClientIp ?? "Unknown",
                request.UserAgent ?? "Unknown",
                request.DeviceId,
                additionalInfo: "Logout realizado"
            );
            logoutLog.MarkAsSuccessful();

            // Revoga o refresh token específico se fornecido
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var refreshTokenHash = _refreshTokenHasher.Hash(request.RefreshToken);
                var refreshTokenHashVo = new RefreshTokenHash(refreshTokenHash);
                user.RevokeRefreshToken(refreshTokenHashVo);
            }
            else
            {
                // Se não tem refresh token, revoga todos os tokens ativos (logout global)
                user.RevokeAllRefreshTokens();
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            return Result<bool>.Failure(
                Error.Infrastructure(
                    "logout.failed",
                    "Ocorreu um erro durante o logout.",
                    details: ex.Message
                )
            );
        }
    }
}
