using PatrickBotman.Bot.Interfaces;
using Telegram.Bot.Types;

namespace PatrickBotman.Bot.UpdateHandlers
{

    public class ReactionUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<ReactionUpdateHandler> _logger;

        public ReactionUpdateHandler(ILogger<ReactionUpdateHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(Update update)
        {
            _logger.LogInformation($"Recieved reaction {update.MessageReaction!.NewReaction[0]} from {update.MessageReaction.User!.Username}");
        }
    }
}