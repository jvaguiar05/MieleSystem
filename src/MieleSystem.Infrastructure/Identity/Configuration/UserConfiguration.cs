using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;
using MieleSystem.Domain.Identity.Enums;
using MieleSystem.Domain.Identity.ValueObjects;
using Mail = MieleSystem.Domain.Identity.ValueObjects.Email;

namespace MieleSystem.Infrastructure.Identity.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd(); // explícito

        builder.HasIndex(x => x.PublicId).IsUnique();

        // Soft Delete
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

        builder
            .Property(x => x.Email)
            .HasConversion(email => email.Value, value => new Mail(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder
            .Property(x => x.PasswordHash)
            .HasConversion(hash => hash.Value, value => new PasswordHash(value))
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(x => x.Role)
            .HasColumnName("Role")
            .HasConversion(role => role.Value, value => UserRole.FromValue(value))
            .IsRequired();

        builder.Property(x => x.RegistrationSituation).HasConversion<int>().IsRequired();

        builder.Property(x => x.ExpiresAt).HasColumnType("date");

        // ------------- Navegações -------------

        builder
            .HasMany(x => x.RefreshTokens)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.OtpSessions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
