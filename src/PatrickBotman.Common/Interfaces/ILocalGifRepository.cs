using PatrickBotman.Common.DTO;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Interfaces
{
    public interface ILocalGifRepository
    {
        Task CreateGifFileAsync(GifFile file);
        Task DeleteGifFileAsync(int id);
        Task<GifFile> GetGifFileAsync(int id);
        Task<Page<GifFileInfo>> GetGifFilesPageAsync(int pageNumber);
        Task<GifFile> GetRandomGifFileAsync();
        Task<IEnumerable<GifFile>> GetRandomGifFilesAsync(int count);
        Task<bool> IsGifExistsAsync(string name);
        Task<bool> IsGifExistsAsync(int id);
        Task<string> GetGifFileId(int id);

    }
}