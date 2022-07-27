using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WavesCasinoAPI.Areas.CaribbeanStudPoker.Models;
using WavesCasinoAPI.Areas.Roulette.Models;
using WavesCasinoAPI.Areas.State.Models;
using WavesCasinoAPI.Models;

namespace WavesCasinoAPI.Data
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<GameState> GameStates { get; set; }
        public DbSet<RouletteBet> RouletteBets { get; set; }
        public DbSet<RouletteGame> RouletteGames { get; set; }
        public DbSet<CaribbeanStudPokerGame> CaribbeanStudPokerGames { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                 .Entries()
                 .Where(e => e.Entity is BaseEntity && (
                         e.State == EntityState.Added
                         || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).LastModifiedOn = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            var entries = ChangeTracker
                 .Entries()
                 .Where(e => e.Entity is BaseEntity && (
                         e.State == EntityState.Added
                         || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).LastModifiedOn = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                 .Entries()
                 .Where(e => e.Entity is BaseEntity && (
                         e.State == EntityState.Added
                         || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).LastModifiedOn = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        public void DetachAllEntities()
        {
            foreach (EntityEntry dbEntityEntry in this.ChangeTracker.Entries().ToArray())
            {

                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }
    }
}
