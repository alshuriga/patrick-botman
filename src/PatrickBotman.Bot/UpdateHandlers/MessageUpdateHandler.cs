using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace PatrickBotman.Bot.UpdateHandlers
{

    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<MessageUpdateHandler> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly AnimationComposeService _edit;
        private readonly IGifService _gifService;
        private readonly IGifProvider _gifProvider;
        private readonly BotConfiguration _options;


        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationComposeService edit,
            IGifService gifService,
            IGifProvider gifProvider,
            IOptions<BotConfiguration> options
            )
        {
            _botClient = botClient;
            _edit = edit;
            _gifService = gifService;
            _gifProvider = gifProvider;
            _logger = logger;
            _options = options.Value;
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message!;

            _logger.LogInformation($"Recieved a message from user {msg.From?.Id}");

            var entityValues = msg.EntityValues ?? Enumerable.Empty<string>();

            if (entityValues.Contains("/gif") || (msg.Chat.Type == ChatType.Private && !entityValues.Contains("/add")))
            {

                var txtSource = msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup ?
               msg.ReplyToMessage : msg;

                var messageText = txtSource?.Caption ?? txtSource?.Text;

                if (messageText == null) throw new Exception("text is null");

                var gif = await _gifProvider.RandomGifAsync(msg.Chat.Id);

                var tgFile = await _edit.ComposeGifAsync(gif, messageText);

                if (gif.Type != GifType.Local)
                {
                    await _botClient.SendAnimationAsync(
              replyMarkup: InlineKeyboard.CreateVotingInlineKeyboard(gif.Id),
              chatId: msg.Chat.Id,
              animation: tgFile);
                }

            }

            else if (entityValues.Any(ev => ev.Contains("/add")))
            {
                if (msg.ReplyToMessage?.Animation == null) throw new Exception("⚠️ No animation found, unable to add a gif");

                if (!_options.AdminID.Split(' ').ToList().Contains(msg.From!.Id.ToString()))
                {
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id,
                        replyToMessageId: msg.MessageId,
                        allowSendingWithoutReply: true,
                        text: "🚫 You dont have a rights to add new gifs");

                    return;
                }
                //else if(msg.ReplyToMessage.Animation.FileSize > 500_000 || msg.ReplyToMessage.Animation.Duration > 3)
                else if (false)
                {
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id,
                        replyToMessageId: msg.MessageId,
                        allowSendingWithoutReply: true,
                        text: "⚠️ The file is too large or too long.  Maximum size is 500KB and maximum length is 3s");
                    return;
                }

                using var stream = new MemoryStream();
                var animFile = await _botClient.GetFileAsync(msg.ReplyToMessage.Animation.FileId);
                await _botClient.DownloadFileAsync(animFile.FilePath!, stream);
                var bytes = stream.ToArray();

                await _gifService.AddNewGifFileAsync(new Common.DTO.GifFileDTO(animFile.FileUniqueId, bytes));

                await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id, replyToMessageId: msg.MessageId, allowSendingWithoutReply: true, text: "✅ Gif was successfully added to the collection.");
            }
        }


    }

}

