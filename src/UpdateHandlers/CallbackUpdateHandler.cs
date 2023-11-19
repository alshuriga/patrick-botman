using patrick_botman.Helpers;
using PatrickBotman.Interfaces;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace patrick_botman.UpdateHandlers
{

    public class CallbackUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IGifRepository _gifRepository;


        public CallbackUpdateHandler(ITelegramBotClient botClient,
            IGifRepository gifRepository,
            ILogger<HandleUpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
            _gifRepository = gifRepository;
        }

        public async Task HandleAsync(Update update)
        {
            var callbackQuery = update.CallbackQuery!;

            if (callbackQuery.Message?.MessageId == null || callbackQuery.Message?.Chat.Id == null || callbackQuery.Data == null) return;

            var chatId = callbackQuery.Message.Chat.Id;

            if ((callbackQuery.Data.StartsWith("blacklist")))
            {
                var gifId = int.Parse(callbackQuery.Data.Split(' ')[1]);

                await _gifRepository.BlacklistAsync(gifId, chatId);

                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"The GIF has been blacklisted 🚮", showAlert: false ,cacheTime: 10);
              
                await _botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(Enumerable.Empty<InlineKeyboardButton>()));
                
                var whoBlacklisted = callbackQuery.From.Username != null ? $"@{callbackQuery.From.Username}" : callbackQuery.From.FirstName;
                await _botClient.EditMessageCaptionAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"🚮 blacklisted by {whoBlacklisted}");
            }
        }
    }
}
