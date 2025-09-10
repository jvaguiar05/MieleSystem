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
            .ForMember(d => d.Id, cfg => cfg.MapFrom(s => s.Id))
            .ForMember(d => d.PublicId, cfg => cfg.MapFrom(s => s.PublicId))
            .ForMember(d => d.Code, cfg => cfg.MapFrom(s => s.Otp.Code))
            .ForMember(d => d.ExpiresAtUtc, cfg => cfg.MapFrom(s => s.Otp.ExpiresAt))
            .ForMember(d => d.CreatedAtUtc, cfg => cfg.MapFrom(s => s.CreatedAtUtc))
            .ForMember(d => d.IsUsed, cfg => cfg.MapFrom(s => s.IsUsed))
            .ForMember(d => d.Purpose, cfg => cfg.MapFrom(s => s.Purpose))
            .ForMember(d => d.UsedAtUtc, cfg => cfg.MapFrom(s => s.UsedAtUtc))
            .ForMember(d => d.UserId, cfg => cfg.MapFrom(s => s.UserId));
    }
}
