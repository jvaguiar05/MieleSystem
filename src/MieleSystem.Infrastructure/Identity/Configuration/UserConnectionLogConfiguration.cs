using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Infrastructure.Identity.Configuration;

public sealed class UserConnectionLogConfiguration : IEntityTypeConfiguration<UserConnectionLog>
{
    public void Configure(EntityTypeBuilder<UserConnectionLog> builder)
    {
        builder.ToTable("UserConnectionLogs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => x.PublicId).IsUnique();

        // Propriedades básicas
        builder.Property(x => x.IpAddress).HasMaxLength(50).IsRequired();

        builder.Property(x => x.UserAgent).HasMaxLength(500).IsRequired();

        builder.Property(x => x.DeviceId).HasMaxLength(100).IsRequired(false);

        builder.Property(x => x.Location).HasMaxLength(200).IsRequired(false);

        builder
            .Property(x => x.ConnectedAtUtc)
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(x => x.IsSuccessful).IsRequired();

        builder.Property(x => x.RequiredOtp).IsRequired();

        builder.Property(x => x.OtpReason).HasMaxLength(200).IsRequired(false);

        builder.Property(x => x.AdditionalInfo).HasMaxLength(1000).IsRequired(false);

        // Índices para performance
        builder
            .HasIndex(x => new { x.UserId, x.ConnectedAtUtc })
            .HasDatabaseName("IX_UserConnectionLogs_UserId_ConnectedAtUtc");

        builder
            .HasIndex(x => new { x.IpAddress, x.ConnectedAtUtc })
            .HasDatabaseName("IX_UserConnectionLogs_IpAddress_ConnectedAtUtc");

        builder
            .HasIndex(x => x.ConnectedAtUtc)
            .HasDatabaseName("IX_UserConnectionLogs_ConnectedAtUtc");

        // FK para User
        builder.Property(x => x.UserId).IsRequired();
    }
}
