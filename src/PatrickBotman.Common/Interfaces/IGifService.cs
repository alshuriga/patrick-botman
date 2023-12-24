using PatrickBotman.Common.DTO;

namespace PatrickBotman.Common.Interfaces;

public interface IGifService
{
    Task<bool> IsBlacklistedAsync(string gifUrl, long chatId);
    Task BlacklistAsync(int id, long chatId);
    Task<int> GetIdOrCreateAsync(string gifUrl);

    Task<Page<GifDTO>> GetBlacklistedGifsPageAsync(int pageNumber, long chatId);
}