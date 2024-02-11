using PatrickBotman.Bot.Models;

namespace PatrickBotman.Bot.Interfaces;

public interface IGifProvider
{
<<<<<<< Updated upstream
    public Task<Gif> RandomGifAsync(long chatId);
=======
    public Task<GifFileWithType> RandomGifAsync(long chatId);

    public Task<IEnumerable<GifFileWithType>> RandomPreviewsAsync(int count);

    public Task<GifFileWithType> GetByIdAsync(int id, GifType type);

    Task<bool> IsLocalExistsAsync(string fileId);
>>>>>>> Stashed changes
}