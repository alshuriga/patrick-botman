using Microsoft.EntityFrameworkCore;
using patrick_botman.Persistence.Entities;
using PatrickBotman.Interfaces;
using PatrickBotman.Persistence;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Services;

public class GifRepository : IGifRepository
{
    private readonly GifRatingsContext _context;

    public GifRepository(GifRatingsContext context)
    {
        _context = context;
    }

    public async Task<bool> IsBlacklistedAsync(string gifUrl, long chatId)
    {
        var isBlacklisted = await _context.Blacklists.Include(b => b.Gif).AnyAsync(b => b.Gif.GifUrl == gifUrl && b.ChatId == chatId);

        return (isBlacklisted);
    }

    public async Task BlacklistAsync(int gifId, long chatId)
    {
        var gif = await _context.Gifs.Include(g => g.Blacklist).SingleAsync(g => g.Id == gifId);   

        if(!gif.Blacklist.Any(b => b.ChatId == chatId))
        {
            gif.Blacklist.Add(new Blacklist()
            {
                ChatId = chatId
            });
            await _context.SaveChangesAsync();  
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetIdOrCreateAsync(string gifUrl)
    {
        var gif = await _context.Gifs.SingleOrDefaultAsync(g => g.GifUrl == gifUrl);
        if (gif == null)
        {
            gif = new Gif() { GifUrl = gifUrl };
            _context.Gifs.Add(gif);
            await _context.SaveChangesAsync();
        }

        return gif.Id;
    }
}
