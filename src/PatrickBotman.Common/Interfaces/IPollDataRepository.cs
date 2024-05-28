using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Interfaces
{
    public interface IPollDataRepository
    {
        Task AddPollDataAsync(PollData pollData);
        Task<PollData> GetPollDataAsync(string pollId);
        Task<bool> IsPollDataExists(int gifId);
        Task RemovePollDataAsync(string pollId);
    }
}