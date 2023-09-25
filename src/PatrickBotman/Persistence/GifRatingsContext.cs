using Microsoft.EntityFrameworkCore;
using PatrickBotman.Persistence.Entities;
using Telegram.Bot.Requests;

namespace PatrickBotman.Persistence;

public class GifRatingsContext : DbContext
{
    public DbSet<GifRating> GifRatings {get; set;}

    public DbSet<Gif> Gifs { get; set; }

    public GifRatingsContext(DbContextOptions<GifRatingsContext> dbContextOptions) : base(dbContextOptions)
    {}
}