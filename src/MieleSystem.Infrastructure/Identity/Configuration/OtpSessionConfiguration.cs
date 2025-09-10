using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Infrastructure.Identity.Configuration;

public sealed class OtpSessionConfiguration : IEntityTypeConfiguration<OtpSession>
{
    public void Configure(EntityTypeBuilder<OtpSession> builder)
    {
        builder.ToTable("OtpSessions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.Property(x => x.IsUsed).IsRequired();

        builder.Property(x => x.Purpose).HasConversion<int>().IsRequired();

        builder
            .Property(x => x.CreatedAtUtc)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(x => x.UsedAtUtc).HasColumnType("timestamp without time zone");

        // ---------------------------
        // Value Object: OtpCode
        // ---------------------------
        builder.OwnsOne(
            x => x.Otp,
            otp =>
            {
                otp.Property(x => x.Code).HasColumnName("OtpCode").HasMaxLength(6).IsRequired();

                otp.Property(x => x.ExpiresAt)
                    .HasColumnName("OtpExpiresAt")
                    .HasColumnType("timestamp without time zone")
                    .IsRequired();
            }
        );

        // FK para User
        builder.Property(x => x.UserId).IsRequired();
    }
}
