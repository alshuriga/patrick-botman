using PatrickBotman.Common.Models;

namespace PatrickBotman.Common.Interfaces
{
    public interface ISettingsRepository
    {
        Task<BotSettings> GetSettings();
        Task UpdateSettings(BotSettings settings);
    }
}