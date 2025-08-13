using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MieleSystem.Domain.Common.Interfaces;

// using MieleSystem.Domain.Identity.Entities;   // descomente quando tiver as entidades
// using MieleSystem.Domain.Clients.Entities;    // idem
// using MieleSystem.Domain.Perdcomps.Entities;  // idem

namespace MieleSystem.Infrastructure.Common.Persistence;

/// <summary>
/// DbContext principal do MieleSystem (monólito modular).
/// Mapeia entidades de todos os Bounded Contexts, mantendo os mapeamentos organizados por contexto.
/// </summary>
internal sealed class MieleDbContext : DbContext
{
    public MieleDbContext(DbContextOptions<MieleDbContext> options)
        : base(options) { }

    // =======================
    // DbSets por contexto
    // =======================

    // Identity
    // public DbSet<User> Users => Set<User>();
    // public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    // public DbSet<OtpSession> OtpSessions => Set<OtpSession>();

    // Clients
    // public DbSet<Client> Clients => Set<Client>();
    // ...

    // Perdcomps
    // public DbSet<Perdcomp> Perdcomps => Set<Perdcomp>();
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações IEntityTypeConfiguration<> do assembly da Infrastructure
        // (crie as classes de mapeamento por contexto em Infrastructure/[Contexto]/Persistence/Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MieleDbContext).Assembly);

        // Aplica filtro global para Soft Delete quando a entidade implementar ISoftDeletable
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Converte deleções em Soft Delete quando a entidade implementa ISoftDeletable
        HandleSoftDeleteChangeTracker(ChangeTracker);

        return await base.SaveChangesAsync(cancellationToken);
    }

    // ==============
    // Helpers
    // ==============
    private static void HandleSoftDeleteChangeTracker(ChangeTracker changeTracker)
    {
        foreach (var entry in changeTracker.Entries().Where(e => e.State == EntityState.Deleted))
        {
            if (entry.Entity is ISoftDeletable soft)
            {
                entry.State = EntityState.Modified;
                soft.Delete(); // chama a lógica encapsulada da entidade
            }
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                // Cria expressão: (e) => !e.IsDeleted
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var notDeleted = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(notDeleted, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
