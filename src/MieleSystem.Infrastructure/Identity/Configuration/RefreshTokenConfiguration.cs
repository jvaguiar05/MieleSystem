using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Infrastructure.Identity.Configuration;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => x.PublicId).IsUnique();

        builder
            .Property(x => x.CreatedAtUtc)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder
            .Property(x => x.ExpiresAtUtc)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc).HasColumnType("timestamp without time zone");

        // ---------------------------
        // Value Object: RefreshTokenHash
        // ---------------------------
        builder.OwnsOne(
            x => x.TokenHash,
            tokenHash =>
            {
                tokenHash
                    .Property(t => t.Value)
                    .HasColumnName("TokenValue")
                    .HasMaxLength(255)
                    .IsRequired();
            }
        );

        // Índices para performance
        builder
            .HasIndex(x => new { x.UserId, x.CreatedAtUtc })
            .HasDatabaseName("IX_RefreshTokens_UserId_CreatedAtUtc");

        // FK para User
        builder.Property(x => x.UserId).IsRequired();
    }
}
