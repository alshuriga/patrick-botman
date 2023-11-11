using PatrickBotman.Interfaces;
using Telegram.Bot.Types.Enums;

namespace patrick_botman.UpdateHandlers
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
                UpdateType.Unknown => throw new ArgumentException("Update type not recognized")
            };
        }
    }
}
