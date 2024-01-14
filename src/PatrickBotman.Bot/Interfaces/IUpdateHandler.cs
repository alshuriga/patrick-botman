using Telegram.Bot.Types;

namespace PatrickBotman.Bot.Interfaces
{
    public interface IUpdateHandler
    {
        Task HandleAsync(Update update);
    }
}
