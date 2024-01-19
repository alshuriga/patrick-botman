using PatrickBotman.Common.DTO;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Interfaces
{
    public interface IOnlineGifRepository
    {
        Task BlacklistAsync(int gifId, long chatId);
        Task UnblacklistAsync(int gifId, long chatId);
        Task<Page<GifDTO>> GetBlacklistedGifsPageAsync(int pageNumber, long chatId);
        Task<int> GetIdOrCreateAsync(string gifUrl);
        Task<bool> IsBlacklistedAsync(string gifUrl, long chatId);
        Task<Gif> GetByIdAsync(int id);
    }
}