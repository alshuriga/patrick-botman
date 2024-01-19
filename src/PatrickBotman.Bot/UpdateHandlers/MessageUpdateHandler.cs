using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
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
        private readonly IOnlineGifRepository _onlineGifRepo;
        private readonly ILocalGifRepository _localGifRepo;
        private readonly IGifProvider _gifProvider;
        private readonly BotConfiguration _options;


        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationComposeService edit,
            IOnlineGifRepository gifService,
            IGifProvider gifProvider,
            IOptionsSnapshot<BotConfiguration> options
,
            ILocalGifRepository localGifRepo)
        {
            _botClient = botClient;
            _edit = edit;
            _onlineGifRepo = gifService;
            _gifProvider = gifProvider;
            _logger = logger;
            _options = options.Value;
            _localGifRepo = localGifRepo;
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message!;

            _logger.LogInformation($"Recieved a message from user {msg.From?.Id}");

            var entityValues = msg.EntityValues ?? Enumerable.Empty<string>();

            if (entityValues.Any(ev => ev.Contains("/gif")) || (msg.Chat.Type == ChatType.Private && !entityValues.Any(ev => ev.Contains("/add"))))
            {

                var txtSource = msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup ?
               msg.ReplyToMessage : msg;

                var messageText = txtSource?.Caption ?? txtSource?.Text;

                if (messageText == null) throw new Exception("text is null");

                var gif = await _gifProvider.RandomGifAsync(msg.Chat.Id);

                var tgFile = await _edit.ComposeGifAsync(gif, messageText);

                await _botClient.SendAnimationAsync(
                replyMarkup: gif.Type != GifType.Local ? InlineKeyboard.CreateVotingInlineKeyboard(gif.Id) : null,
                chatId: msg.Chat.Id,
                animation: tgFile);

            }

            else if (entityValues.Any(ev => ev.Contains("/add")))
            {
                if (msg.ReplyToMessage?.Animation == null) throw new Exception("⚠️ No animation found, unable to add a gif");

                if (!_options.AdminID.Split(' ').ToList().Contains(msg.From!.Id.ToString()))
                {
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id,
                        replyToMessageId: msg.MessageId,
                        allowSendingWithoutReply: true,
                        text: "🚫 You dont have rights to add new gifs");

                    return;
                }
                //else if(msg.ReplyToMessage.Animation.FileSize > 500_000 || msg.ReplyToMessage.Animation.Duration > 3)
                //{
                //    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id,
                //        replyToMessageId: msg.MessageId,
                //        allowSendingWithoutReply: true,
                //        text: "⚠️ The file is too large or too long.  Maximum size is 500KB and maximum length is 3s");
                //    return;
                //}

                using var stream = new MemoryStream();
                var animFile = await _botClient.GetFileAsync(msg.ReplyToMessage.Animation.FileId);
                await _botClient.DownloadFileAsync(animFile.FilePath!, stream);
                var bytes = stream.ToArray();

                await _localGifRepo.CreateGifFileAsync(new GifFile()
                {
                    Name = animFile.FileId,
                    Data = bytes
                });

                await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id, replyToMessageId: msg.MessageId, allowSendingWithoutReply: true, text: "✅ Gif was successfully added to the collection.");
            }
        }


    }

}

