using AutoMapper;
using MieleSystem.Application.Identity.DTOs;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Application.Identity.Mappings;

public sealed class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, UserListItemDto>()
            .ForMember(d => d.PublicId, cfg => cfg.MapFrom(s => s.PublicId))
            .ForMember(d => d.Name, cfg => cfg.MapFrom(s => s.Name))
            .ForMember(d => d.Email, cfg => cfg.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Role, cfg => cfg.MapFrom(s => s.Role.ToString()));

        CreateMap<RefreshToken, RefreshTokenDto>()
            .ForMember(d => d.UserId, cfg => cfg.Ignore())
            .ForMember(d => d.TokenHash, cfg => cfg.MapFrom(s => s.TokenHash.Value));

        CreateMap<UserConnectionLog, UserConnectionLogDto>()
            .ForMember(d => d.UserId, cfg => cfg.Ignore());

        CreateMap<OtpSession, OtpSessionDto>()
            .ForMember(d => d.Code, cfg => cfg.MapFrom(s => s.Otp.Code))
            .ForMember(d => d.ExpiresAtUtc, cfg => cfg.MapFrom(s => s.Otp.ExpiresAt))
            .ForMember(d => d.UserId, cfg => cfg.Ignore());
    }
}
