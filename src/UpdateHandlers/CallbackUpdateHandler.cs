using PatrickBotman.Interfaces;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace patrick_botman.UpdateHandlers
{

    public class CallbackUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IGifRatingService _gifRatingRepository;


        public CallbackUpdateHandler(ITelegramBotClient botClient,
            IGifRatingService gifRatingRepository,
            ILogger<HandleUpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
            _gifRatingRepository = gifRatingRepository;
        }

        public async Task HandleAsync(Update update)
        {
            var callbackQuery = update.CallbackQuery!;

            if (callbackQuery.Message?.MessageId == null || callbackQuery.Message?.Chat.Id == null || callbackQuery.Data == null) return;

            var userId = callbackQuery.From.Id;
            var chatId = callbackQuery.Message.Chat.Id;

            if (!(callbackQuery.Data.StartsWith("up") || callbackQuery.Data.StartsWith("down"))) return;

            var voteMode = callbackQuery.Data.StartsWith("up") ? true : false;

            var gifId = int.Parse(callbackQuery.Data.Split(' ')[1]);

            await _gifRatingRepository.RateGifAsync(voteMode, gifId, userId, chatId);

            var rating = await _gifRatingRepository.GetGifRatingByIdAsync(int.Parse(callbackQuery.Data.Split(' ')[1]), chatId);

            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"New rating is {rating}");
            // await _botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, CreateVotingInlineKeyboard(rating, int.Parse(callbackQuery.Data.Split(' ')[1])));       
        }
    }
}
