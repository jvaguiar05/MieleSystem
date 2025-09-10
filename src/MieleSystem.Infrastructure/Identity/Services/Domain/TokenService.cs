using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Services;
using MieleSystem.Infrastructure.Identity.Options;

namespace MieleSystem.Infrastructure.Identity.Services.Domain;

/// <summary>
/// Implementação do serviço de geração de tokens JWT e refresh tokens.
/// </summary>
public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtOptions _options = options.Value;

    public string GenerateAccessToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_options.Secret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
            new(ClaimTypes.Email, user.Email.ToString()),
            new(ClaimTypes.Role, user.Role.Name),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiration.TotalMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public DateTime GetAccessTokenExpiration() =>
        DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiration.TotalMinutes);

    public DateTime GetRefreshTokenExpiration() =>
        DateTime.UtcNow.AddDays(_options.RefreshTokenExpiration.TotalDays);
}
