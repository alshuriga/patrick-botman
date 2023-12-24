using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Persistence;

public class GifRatingsContext : DbContext
{
    public DbSet<Gif> Gifs { get; set; } = null!;

    public DbSet<Blacklist> Blacklists { get; set; } = null!;

    public GifRatingsContext(DbContextOptions<GifRatingsContext> dbContextOptions) : base(dbContextOptions)
    {}
}