using Telegram.Bot.Types;

namespace PatrickBotman.Interfaces
{
    public interface IUpdateHandler
    {
        Task HandleAsync(Update update);
    }
}
