using PatrickBotman.Interfaces;
using PatrickBotman.Models;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace patrick_botman.UpdateHandlers
{

    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<MessageUpdateHandler> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly AnimationEditService _edit;
        private readonly IGifRatingRepository _gifRatingRepository;
        private readonly IGifService _gifService;


        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationEditService edit,
            IGifRatingRepository gifRatingRepository,
            IGifService gifService)
        {
            _botClient = botClient;
            _edit = edit;
            _gifRatingRepository = gifRatingRepository;
            _gifService = gifService;
            _logger = logger;
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message;

            _logger.LogInformation($"Recieved '{msg.Text}' message from user Id '{msg.From?.Id}.'");
            string? messageText = msg.Caption ?? msg.Text;


            if (messageText == null) return;

            //var botUserName = (await _botClient.GetMeAsync()).Username;

            if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
            {
                if (!messageText.Contains($"/gif")) return;
                messageText = msg.ReplyToMessage?.Caption ?? msg.ReplyToMessage?.Text;
                if (messageText == null) return;
            }

            var isFavoriteGif = new Random().Next(0, 100) > 30;
            GifDTO? gifDTO = null;

            if (isFavoriteGif)
            {
                gifDTO = await _gifRatingRepository.GetRandomGifAsync(msg.Chat.Id);
            }
            if (!isFavoriteGif || gifDTO == null)
            {
                var url = await _gifService.RandomTrendingAsync();
                var id = await _gifRatingRepository.GetGifIdAsync(url);
                gifDTO = new GifDTO(id, url);
            }

            var rating = await _gifRatingRepository.GetGifRatingAsync(gifDTO.GifId, msg.Chat.Id);

            var file = await _edit.AddText(gifDTO.GifUrl, messageText);

            if (file != null)
            {
                await using (var stream = System.IO.File.OpenRead(file))
                {
                    stream.Position = 0;
                    await _botClient.SendAnimationAsync(
                                replyMarkup: CreateVotingInlineKeyboard(gifDTO.GifId),
                                chatId: msg.Chat.Id,
                                animation: new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4"));
                }
            }

        }

        private InlineKeyboardMarkup CreateVotingInlineKeyboard(int gifId)
        {
            IEnumerable<InlineKeyboardButton> buttons = new[] { InlineKeyboardButton.WithCallbackData($"👎",  $"down {gifId}"),
            InlineKeyboardButton.WithCallbackData($"👍", $"up {gifId}")};

            var replyMarkup = new InlineKeyboardMarkup(buttons);

            return replyMarkup;
        }
    }
}
