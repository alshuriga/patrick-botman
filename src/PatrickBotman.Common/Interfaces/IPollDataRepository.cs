using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Interfaces
{
    public interface IPollDataRepository
    {
        Task AddPollDataAsync(PollData pollData);
        Task<PollData> GetPollDataAsync(string gifId, string chatId);
        Task<bool> IsPollDataExists(string gifId, string chatId);
        Task RemovePollDataAsync(string gifId, string chatId);
        Task<PollData> AddVoteToPollAsync(string gifId, string chatId, string userId, int vote);
    }
}