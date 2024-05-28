using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Persistence;

public class PatrickBotmanContext : DbContext
{
    public DbSet<Gif> Gifs { get; set; } = null!;

    public DbSet<Blacklist> Blacklists { get; set; } = null!;

    public DbSet<GifFile> GifFiles { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<PollData> PollData { get; set; } = null!;

    public DbSet<ConfigEntry> ConfigEntries { get; set; } = null!;

    public PatrickBotmanContext(DbContextOptions<PatrickBotmanContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GifFile>().HasQueryFilter(x => x.Deleted == false);
        base.OnModelCreating(modelBuilder);

    }

}

public class SoftDeleteInterceptor: SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        foreach (var entry in eventData.Context!.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: IDeletableEntity delete }) continue;

            entry.State = EntityState.Modified;
            delete.Deleted = true;
        }
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await ValueTask.FromResult(result);
        }

        foreach (var entry in eventData.Context!.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: IDeletableEntity delete }) continue;

            entry.State = EntityState.Modified;
            delete.Deleted = true;
        }
        return await ValueTask.FromResult(result);
    }
}