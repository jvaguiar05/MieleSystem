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
            .ForMember(d => d.Id, cfg => cfg.MapFrom(s => s.Id))
            .ForMember(d => d.PublicId, cfg => cfg.MapFrom(s => s.PublicId))
            .ForMember(d => d.TokenHash, cfg => cfg.MapFrom(s => s.TokenHash.Value))
            .ForMember(d => d.ExpiresAtUtc, cfg => cfg.MapFrom(s => s.ExpiresAtUtc))
            .ForMember(d => d.IsRevoked, cfg => cfg.MapFrom(s => s.IsRevoked))
            .ForMember(d => d.RevokedAtUtc, cfg => cfg.MapFrom(s => s.RevokedAtUtc))
            .ForMember(d => d.CreatedAtUtc, cfg => cfg.MapFrom(s => s.CreatedAtUtc))
            .ForMember(d => d.UserId, cfg => cfg.Ignore()); // UserId será preenchido pelo EF Core
    }
}
