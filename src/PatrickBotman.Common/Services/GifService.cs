using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.DTO;
using System.ComponentModel.DataAnnotations;
using PatrickBotman.Common;

namespace PatrickBotman.Services;

public class GifService : IGifService
{
    private readonly PatrickBotmanContext _context;

    public GifService(PatrickBotmanContext context)
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

    public async Task<Page<GifDTO>> GetBlacklistedGifsPageAsync(int pageNumber, long chatId)
    {
        return new Page<GifDTO>() { 
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

    public async Task<Page<ChatDTO>> GetChatsPageAsync(int pageNumber)
    {
        return new Page<ChatDTO>()
        {
            Items = await _context.Blacklists
            .OrderBy(b => b.Id)
            .GroupBy(b => b.ChatId)
            .Select(c => c.First().ChatId)
            .Skip(pageNumber * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE)
            .Select(c => new ChatDTO(c))
            .ToListAsync(),

            CollectionSize = await _context.Blacklists
                .GroupBy(b => b.ChatId)
                .CountAsync()
        };
    }

    public async Task AddNewGifFileAsync(GifFileDTO gifFile)
    {
        await _context.GifFiles.AddAsync(new GifFile()
        {
            Name = gifFile.fileName,
            Data = gifFile.data
        });

        await _context.SaveChangesAsync();
    }

    public async Task<GifFileDTO> GetGifFileAsync(int id)
    {
        var file = await _context.GifFiles.FirstAsync(f => f.Id == id);

        return new GifFileDTO(file.Name, file.Data);
    }
}
