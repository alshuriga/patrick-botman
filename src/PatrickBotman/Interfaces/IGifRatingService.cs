using PatrickBotman.Models;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Interfaces;

public interface IGifRatingService
{
    Task<GifDTO?> GetRandomGifAsync(long chatId);
    Task<int> GetGifRatingByIdAsync(int gifId, long chatId);
    Task<string> GetUrlById(int gifId);
    Task<int> GetOrCreateIdForGifUrlAsync(string gifUrl);
    Task RateGifAsync(bool upvote, int gifId, long userId, long chatId);
}