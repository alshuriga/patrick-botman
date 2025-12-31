using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Services
{
    public class PollDataRepository : IPollDataRepository
    {
        private readonly PatrickBotmanContext _context;

        public PollDataRepository(PatrickBotmanContext context)
        {
            _context = context;
        }

        public async Task<PollData> GetPollDataAsync(string gifId, string chatId)
        {
            return await _context.PollData.SingleAsync(p => p.PollChatId == chatId && p.GifFileId.ToString() == gifId && p.Closed == false);
        }

        public async Task AddPollDataAsync(PollData pollData)
        {
            await _context.PollData.AddAsync(pollData);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePollDataAsync(string gifId, string chatId)
        {
            var pollData = await _context.PollData.SingleAsync(p => p.PollChatId == chatId && p.GifFileId.ToString() == gifId);

            _context.PollData.Remove(pollData);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPollDataExists(string gifId, string chatId)
        {
            return await _context.PollData.AnyAsync(p => p.PollChatId == chatId && p.GifFileId.ToString() == gifId && p.Closed == false);
        }

        public async Task<PollData> AddVoteToPollAsync(string gifId, string chatId, string userId, int vote)
        {
            var pollData = await _context.PollData.Include(pd => pd.PollVote).SingleAsync(p => p.PollChatId == chatId.ToString() && p.GifFileId.ToString() == gifId && p.Closed == false);

            var existingVote = pollData.PollVote.FirstOrDefault(v => v.UserId == long.Parse(userId));

            if (existingVote != null)
            {
                existingVote.Vote = vote;
                _context.Update(existingVote);
            }
            else
            {
                var newVote = new PollVote()
                {
                    Vote = vote,
                    PollData = pollData,
                    UserId = long.Parse(userId),
                };

                pollData.PollVote.Add(newVote);
                _context.PollVote.Add(newVote);
            }

            await _context.SaveChangesAsync();

            return pollData;
        }

        public async Task<IEnumerable<PollData>> GetOpenPollsAsync()
        {
            var polls = await _context.PollData.Include(p => p.PollVote).Where(p => p.Closed == false).ToListAsync();
            return polls;
        }

        public async Task ClosePollsByIds(IEnumerable<int> ids)
        {
            var polls = await _context.PollData.Where(p => ids.Contains(p.Id)).ToListAsync();

            foreach(var poll in polls)
            {
                poll.Closed = true;
            };

            await _context.SaveChangesAsync();
        }
    }
}
