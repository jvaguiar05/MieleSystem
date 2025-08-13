using MieleSystem.Application.Common.DTOs;
using MieleSystem.Application.Common.Responses;

namespace MieleSystem.Application.Common.Extensions;

public static class ResultExtensions
{
    public static ResultDto<T> ToDto<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? ResultDto<T>.Ok(result.Value!)
            : ResultDto<T>.Fail(result.Error ?? "Erro desconhecido.");
    }

    public static ResultDto ToDto(this Result result)
    {
        return result.IsSuccess
            ? ResultDto.Ok()
            : ResultDto.Fail(result.Error ?? "Erro desconhecido.");
    }
}
