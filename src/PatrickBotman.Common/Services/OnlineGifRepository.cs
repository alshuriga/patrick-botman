using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.DTO;
using PatrickBotman.Common;
using PatrickBotman.Common.Interfaces;

namespace PatrickBotman.Services;

public class OnlineGifRepository : IOnlineGifRepository
{
    private readonly PatrickBotmanContext _context;

    public OnlineGifRepository(PatrickBotmanContext context)
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

        if (!gif.Blacklist.Any(b => b.ChatId == chatId))
        {
            gif.Blacklist.Add(new Blacklist()
            {
                ChatId = chatId
            });
            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();
    }

    public async Task UnblacklistAsync(int gifId, long chatId)
    {
        var entry = await _context.Blacklists
            .Include(b => b.Gif)
            .SingleAsync(b => b.ChatId == chatId && b.Gif.Id == gifId);

        _context.Blacklists.Remove(entry);

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

    public async Task<Page<GifDTO>> GetBlacklistedGifsPageAsync(int pageNumber, long chatId)
    {
        return new Page<GifDTO>()
        {
            Items = await _context.Blacklists
                .Include(b => b.Gif)
                .Where(b => b.ChatId == chatId)
                .OrderBy(b => b.Id)
                .Skip(pageNumber * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
                .Select(b => new GifDTO(b.Gif.Id, b.Gif.GifUrl))
                .ToListAsync(),
            CollectionSize = await _context.Blacklists
                .Where(b => b.ChatId == chatId)
                .CountAsync()
        };
    }

    public async Task<Gif> GetByIdAsync(int id)
    {
        return await _context.Gifs.FirstAsync(b => b.Id == id);
    }
}
