using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Persistence;

public class PatrickBotmanContext : DbContext
{
    public DbSet<Gif> Gifs { get; set; } = null!;

    public DbSet<Blacklist> Blacklists { get; set; } = null!;

    public DbSet<GifFile> GifFiles { get; set; } = null!;

    public PatrickBotmanContext(DbContextOptions<PatrickBotmanContext> dbContextOptions) : base(dbContextOptions)
    {}
}