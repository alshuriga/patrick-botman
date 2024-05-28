using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.DTO;
using PatrickBotman.Common;

namespace PatrickBotman.Services;

public class ChatsRepository : IChatsRepository
{
    private readonly PatrickBotmanContext _context;

    public ChatsRepository(PatrickBotmanContext context)
    {
        _context = context;
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
}
