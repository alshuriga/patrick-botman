using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using PatrickBotman.Bot.UpdateHandlers;

namespace PatrickBotman.Services;

public class HandleUpdateService
{
    private readonly UpdateHandlersFactory _updateHandlersFactory;
    private readonly ILogger<HandleUpdateService> _logger;
    public HandleUpdateService(UpdateHandlersFactory updateHandlersFactory, ILogger<HandleUpdateService> logger)
    {
        _updateHandlersFactory = updateHandlersFactory;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        var handler = _updateHandlersFactory.Create(update.Type);
        try
        {
            await handler.HandleAsync(update);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception:\n{ex.Message}\n{ex.InnerException?.Message}\n{ex.StackTrace}");
        }
    }
}