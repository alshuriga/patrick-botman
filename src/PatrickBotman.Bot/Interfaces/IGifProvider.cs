using PatrickBotman.Bot.Models;

namespace PatrickBotman.Bot.Interfaces;

public interface IGifProvider
{
    public Task<GifFileWithType> RandomGifAsync(long chatId);

    public Task<IEnumerable<GifFileWithType>> RandomPreviewsAsync(int count);

    public Task<GifFileWithType> GetByIdAsync(int id, GifType type);
}