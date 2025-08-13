using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MieleSystem.Domain.Common.Interfaces;
// Contextos
using MieleSystem.Domain.Identity.Entities;

// using MieleSystem.Domain.Clients.Entities;
// using MieleSystem.Domain.Perdcomps.Entities;

namespace MieleSystem.Infrastructure.Common.Persistence;

/// <summary>
/// DbContext principal do MieleSystem (monólito modular).
/// Mapeia entidades de todos os Bounded Contexts, mantendo os mapeamentos organizados por contexto.
/// </summary>
public sealed class MieleDbContext(DbContextOptions<MieleDbContext> options) : DbContext(options)
{
    // =======================
    // DbSets por contexto
    // =======================

    // Identity

    public DbSet<User> Users => Set<User>();
    public DbSet<OtpSession> OtpSessions => Set<OtpSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserAuditLog> UserAuditLogs => Set<UserAuditLog>();

    // Clients
    // public DbSet<Client> Clients => Set<Client>();
    // ...

    // Perdcomps
    // public DbSet<Perdcomp> Perdcomps => Set<Perdcomp>();
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações IEntityTypeConfiguration<> disponíveis neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MieleDbContext).Assembly);

        // Aplica filtros globais para soft delete
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDeleteChangeTracker(ChangeTracker);
        return await base.SaveChangesAsync(cancellationToken);
    }

    // =======================
    // Helpers
    // =======================

    private static void HandleSoftDeleteChangeTracker(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(e => e.State == EntityState.Deleted))
        {
            if (entry.Entity is ISoftDeletable soft)
            {
                entry.State = EntityState.Modified;
                soft.Delete(); // lógica encapsulada
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var notDeleted = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(notDeleted, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
