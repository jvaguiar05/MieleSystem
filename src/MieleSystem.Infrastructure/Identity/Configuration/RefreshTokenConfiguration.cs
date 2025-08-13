using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.ValueObjects;

namespace MieleSystem.Infrastructure.Identity.Configuration;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

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
        // Value Object: Token
        // ---------------------------
        builder.OwnsOne(
            x => x.Token,
            token =>
            {
                token
                    .Property(t => t.Value)
                    .HasColumnName("TokenValue")
                    .HasMaxLength(255)
                    .IsRequired();
            }
        );

        // FK para User (privada)
        builder.Property<int>("UserId").IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade);
    }
}
