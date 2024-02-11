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

        public async Task<PollData> GetPollDataAsync(string pollId)
        {
            return await _context.PollData.SingleAsync(p => p.PollId == pollId);
        }

        public async Task AddPollDataAsync(PollData pollData)
        {
            await _context.PollData.AddAsync(pollData);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePollDataAsync(string pollId)
        {
            var pollData = await _context.PollData.SingleAsync(p => p.PollId == pollId);

            _context.PollData.Remove(pollData);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPollDataExists(int gifId)
        {
            return await _context.PollData.AnyAsync(p => p.GifFileId == gifId);
        }
    }
}
