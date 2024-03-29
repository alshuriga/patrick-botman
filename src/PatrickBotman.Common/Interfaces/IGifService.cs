using PatrickBotman.Common.DTO;

namespace PatrickBotman.Common.Interfaces;

public interface IGifService
{
    Task<bool> IsBlacklistedAsync(string gifUrl, long chatId);

    Task BlacklistAsync(int id, long chatId);

    Task<int> GetIdOrCreateAsync(string gifUrl);

    Task<Page<OnlineGifDTO>> GetBlacklistedGifsPageAsync(int pageNumber, long chatId);
    
    Task<Page<LocalGifDTO>> GetLocalGifsPageAsync(int pageNumber);

    Task<Page<ChatDTO>> GetChatsPageAsync(int pageNumber);

    Task AddNewGifFileAsync(GifFileDTO gifFile);

    Task<GifFileDTO> GetGifFileAsync(int id);

    Task<RandomGifFileDTO> GetRandomGifFileAsync();

    Task DeleteGifFileAsync(int id);
}