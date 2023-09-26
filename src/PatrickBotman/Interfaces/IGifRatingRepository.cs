using PatrickBotman.Models;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Interfaces;

public interface IGifRatingRepository
{
    Task<GifDTO?> GetRandomGifAsync(long chatId);
    Task<int> GetGifRatingAsync(int gifId, long chatId);
    Task<string> GetUrlById(int gifId);
    Task<int> GetGifIdAsync(string gifUrl);
    Task RateGifAsync(bool upvote, int gifId, long userId, long chatId);
}