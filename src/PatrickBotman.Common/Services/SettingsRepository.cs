using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Models;
using PatrickBotman.Common.Persistence;

namespace PatrickBotman.Common.Services
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly PatrickBotmanContext _context;

        public SettingsRepository(PatrickBotmanContext context)
        {
            _context = context;
        }

        public async Task<BotSettings> GetSettings()
        {
            var dict = await _context.ConfigEntries.ToDictionaryAsync(k => k.Name.Trim(), v => v.Value);

            return new BotSettings()
            {
                LocalGifProbability = int.Parse(dict["BotConfiguration:LocalGifsProbability"]),
                MaximumTextLength = int.Parse(dict["MaximumTextLength"]),
                AdminID = dict["BotConfiguration:AdminID"].Split(' ')
            };
        }

        public async Task UpdateSettings(BotSettings settings)
        {
            var entries = await _context.ConfigEntries.ToListAsync();

            entries.Single(e => e.Name.Trim() == "BotConfiguration:LocalGifsProbability").Value = settings.LocalGifProbability.ToString();
            entries.Single(e => e.Name.Trim() == "MaximumTextLength").Value = settings.MaximumTextLength.ToString();
            entries.Single(e => e.Name.Trim() == "BotConfiguration:AdminID").Value = string.Join(' ', settings.AdminID);

            await _context.SaveChangesAsync();
        }
    }
}
