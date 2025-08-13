using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MieleSystem.Domain.Identity.Entities;

namespace MieleSystem.Infrastructure.Identity.Configuration;

/// <summary>
/// Configuração da entidade UserAuditLog no EF Core.
/// </summary>
public sealed class UserAuditLogConfiguration : IEntityTypeConfiguration<UserAuditLog>
{
    public void Configure(EntityTypeBuilder<UserAuditLog> builder)
    {
        builder.ToTable("UserAuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Chave estrangeira técnica (sem navegação explícita)
        builder.Property(x => x.UserId).IsRequired();
        builder.HasIndex(x => x.UserId);

        // Identificador público (visível externamente)
        builder.Property(x => x.UserPublicId).IsRequired();
        builder.HasIndex(x => x.UserPublicId);

        builder.Property(x => x.Email).HasMaxLength(255).IsRequired();

        builder.Property(x => x.Action).HasMaxLength(100).IsRequired();

        builder.Property(x => x.OccurredAt).IsRequired();
    }
}
