using PatrickBotman.Bot.Interfaces;
using Telegram.Bot.Types.Enums;

namespace PatrickBotman.Bot.UpdateHandlers
{
    public class UpdateHandlersFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateHandlersFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUpdateHandler Create(UpdateType updateType)
        {
            return updateType switch
            {
                UpdateType.Message => ActivatorUtilities.CreateInstance<MessageUpdateHandler>(_serviceProvider),
                UpdateType.InlineQuery => ActivatorUtilities.CreateInstance<InlineUpdateHandler>(_serviceProvider),
                UpdateType.CallbackQuery => ActivatorUtilities.CreateInstance<CallbackUpdateHandler>(_serviceProvider),
                UpdateType.ChosenInlineResult => ActivatorUtilities.CreateInstance<ChosenInlineUpdateHandler>(_serviceProvider),
                _ => throw new ArgumentException("Update type not recognized")
            };
        }
    }
}
