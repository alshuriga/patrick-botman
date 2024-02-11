using PatrickBotman.Common.DTO;

namespace PatrickBotman.Common.Interfaces
{
    public interface IChatsRepository
    {
        Task<Page<ChatDTO>> GetChatsPageAsync(int pageNumber);
    }
}