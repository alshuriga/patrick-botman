using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.DTO;
using PatrickBotman.Common;

namespace PatrickBotman.Services;

public class LocalGifRepository : ILocalGifRepository
{
    private readonly PatrickBotmanContext _context;
    private readonly System.Random _random;

    public LocalGifRepository(PatrickBotmanContext context)
    {
        _context = context;
        _random = new Random();
    }

    public async Task CreateGifFileAsync(GifFile file)
    {
        await _context.GifFiles.AddAsync(file);
        await _context.SaveChangesAsync();
    }


    public async Task<GifFile> GetGifFileAsync(int id)
    {
        var file = await _context.GifFiles.FirstAsync(f => f.Id == id);

        return file;
    }

    public async Task<GifFile> GetRandomGifFileAsync()
    {
        var count = await _context.GifFiles.CountAsync();
        var randGif = await _context.GifFiles.OrderBy(g => g.Id).Skip(_random.Next(0, count)).Take(1).FirstAsync();

        return randGif;
    }


    public async Task<IEnumerable<GifFile>> GetRandomGifFilesAsync(int count)
    {
        var gifCount = await _context.GifFiles.CountAsync();
        if (count > gifCount || count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        var query = _context.GifFiles.OrderBy(g => g.Id).Skip(_random.Next(0, gifCount)).Take(1);

        for (int i = 0; i < count - 1; i++)
        {
            query = query.Union(_context.GifFiles.OrderBy(g => g.Id).Skip(_random.Next(0, gifCount)).Take(1));
        }

        return await query.ToListAsync();
    }

    public async Task<Page<GifFileInfo>> GetGifFilesPageAsync(int pageNumber)
    {
        return new Page<GifFileInfo>()
        {
            Items = await _context.GifFiles.OrderBy(g => g.Id)
            .Skip(Constants.PAGE_SIZE * pageNumber).Take(Constants.PAGE_SIZE)
            .Select(g => new GifFileInfo(g.Id, g.Data.LongLength / 1024))
            .ToListAsync(),

            CollectionSize = await _context.GifFiles.CountAsync()
        };
    }

    public async Task DeleteGifFileAsync(int id)
    {
        var gif = await _context.GifFiles.FirstAsync(g => g.Id == id);

        _context.GifFiles.Remove(gif);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsGifExistsAsync(string name)
    {
        return await _context.GifFiles.AnyAsync(g => g.Name == name && !g.Deleted);
    }

    public async Task<bool> IsGifExistsAsync(int id)
    {
        return await _context.GifFiles.AnyAsync(g => g.Id == id && !g.Deleted);
    }

    public async Task<string> GetGifFileId(int id)
    {
        return (await _context.GifFiles.Select(g => new { Name = g.Name, Id = g.Id }).SingleAsync(g => g.Id == id)).Name;

    }


}
