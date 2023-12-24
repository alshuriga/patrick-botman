using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using PatrickBotman.Bot.UpdateHandlers;

namespace PatrickBotman.Services;

public class HandleUpdateService
{
    private readonly UpdateHandlersFactory _updateHandlersFactory;
    public HandleUpdateService(UpdateHandlersFactory updateHandlersFactory)
    {
        _updateHandlersFactory = updateHandlersFactory;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        var handler = _updateHandlersFactory.Create(update.Type);

        await handler.HandleAsync(update);
    }
}