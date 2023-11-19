using Microsoft.EntityFrameworkCore;
using patrick_botman.Persistence.Entities;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Persistence;

public class GifRatingsContext : DbContext
{
    public DbSet<Gif> Gifs { get; set; }

    public DbSet<Blacklist> Blacklists { get; set; }

    public GifRatingsContext(DbContextOptions<GifRatingsContext> dbContextOptions) : base(dbContextOptions)
    {}
}