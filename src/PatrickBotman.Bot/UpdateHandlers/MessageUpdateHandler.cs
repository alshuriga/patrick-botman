using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.Services;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
        private readonly IPollDataRepository _pollDataRepository;
        private readonly BotConfiguration _options;
        private readonly IUrlMetaService _urlMetaService;
        private readonly IImageToTextService _imageToTextService;

        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationComposeService edit,
            IOnlineGifRepository gifService,
            IGifProvider gifProvider,
            IOptionsSnapshot<BotConfiguration> options,
            ILocalGifRepository localGifRepo,
            IPollDataRepository pollDataRepository,
            IUrlMetaService urlMetaService,
            IImageToTextService imageToTextService)
        {
            _pollDataRepository = pollDataRepository;
            _botClient = botClient;
            _edit = edit;
            _onlineGifRepo = gifService;
            _gifProvider = gifProvider;
            _logger = logger;
            _options = options.Value;
            _localGifRepo = localGifRepo;
            _urlMetaService = urlMetaService;
            _imageToTextService = imageToTextService;
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message!;

            _logger.LogInformation($"Recieved a message from user {msg.From?.Id}");

            var entityValues = msg.EntityValues ?? Enumerable.Empty<string>();

            if (entityValues.Any(ev => ev.Contains("/ip")) && msg.Chat.Type == ChatType.Private && msg.From!.Id.ToString() == "35306756")
            {
                using (HttpClient client = new HttpClient())
                {
                    var ip = await client.GetStringAsync("https://api.ipify.org");

                    await _botClient.SendMessage(chatId: msg.Chat.Id,
                     replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true },
                     text: ip);

                    return;
                }
            }

            if (entityValues.Any(ev => ev.Contains("/gif")) || (msg.Chat.Type == ChatType.Private
                && !entityValues.Any(ev => ev.Contains("/add"))
                 && !entityValues.Any(ev => ev.Contains("/voteban"))))
            {
                string? messageText = null!;

                if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
                {
                    if (msg.ReplyToMessage?.Photo?.Length > 0 || msg.ReplyToMessage?.Animation?.Thumbnail != null || msg.ReplyToMessage?.Video?.Thumbnail != null)
                    {
                        var photoId = msg.ReplyToMessage?.Photo?[msg.ReplyToMessage.Photo.Length - 1].FileId
                            ?? msg.ReplyToMessage?.Animation?.Thumbnail?.FileId ?? msg.ReplyToMessage?.Video?.Thumbnail?.FileId;

                        if(photoId != null && msg.Quote?.Text == null)
                        {
                            var photoFile = await _botClient.GetFile(photoId!);

                            using var memStream = new MemoryStream();
                            await _botClient.DownloadFile(photoFile.FilePath!, memStream);
                            var imageFile = memStream.ToArray();

                            messageText = _imageToTextService.GetText(imageFile);
                        }
                    }

                    if (messageText == null)
                    {
                        messageText = msg.Quote?.Text ?? msg.ReplyToMessage?.Text ?? msg.ReplyToMessage?.Caption;
                    }
                }
                else
                {
                    messageText = msg.Quote?.Text ?? msg.Text ?? msg.Caption;
                }

                if (messageText == null) throw new Exception("text is null");

                var urlRegex = "https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)";
                var url = Regex.Match(messageText, urlRegex)?.Value;

                if (!string.IsNullOrEmpty(url))
                {
                    messageText = (await _urlMetaService.GetMetaForUrl(new Uri(url))) ?? messageText;
                };

                var gif = await _gifProvider.RandomGifAsync(msg.Chat.Id);

                var tgFile = await _edit.ComposeGifAsync(gif, messageText);

                using var memory = new MemoryStream(tgFile.Data);

                var file = InputFile.FromStream(memory, tgFile.Name);

                await _botClient.SendAnimation(
                replyMarkup: gif.Type != GifType.Local ? InlineKeyboard.CreateVotingInlineKeyboard(gif.Id) : null,
                chatId: msg.Chat.Id,
                animation: file,
                replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true }
                );

            }

            else if (entityValues.Any(ev => ev.Contains("/add")))
            {
                if (msg.ReplyToMessage?.Animation == null) throw new Exception("⚠️ No animation found, unable to add a gif");

                if (!_options.AdminID.Split(' ').ToList().Contains(msg.From!.Id.ToString()))
                {
                    await _botClient.SendMessage(chatId: msg.Chat.Id,
                        replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true },
                        text: "🚫 You dont have rights to add new gifs");

                    return;
                }
                else if (await _localGifRepo.IsGifExistsAsync(msg.ReplyToMessage.Animation.FileId))
                {
                    await _botClient.SendMessage(chatId: msg.Chat.Id,
                        replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true },
                        text: "🚫 The gif is already in the collection");

                    return;
                }
                //else if(msg.ReplyToMessage.Animation.FileSize > 500_000 || msg.ReplyToMessage.Animation.Duration > 3)
                //{
                //    await _botClient.SendMessage(chatId: msg.Chat.Id,
                //        replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true},
                //        allowSendingWithoutReply: true,
                //        text: "⚠️ The file is too large or too long.  Maximum size is 500KB and maximum length is 3s");
                //    return;
                //}

                using var stream = new MemoryStream();
                var animFile = await _botClient.GetFile(msg.ReplyToMessage.Animation.FileId);
                await _botClient.DownloadFile(animFile.FilePath!, stream);
                var bytes = stream.ToArray();

                await _localGifRepo.CreateGifFileAsync(new GifFile()
                {
                    Name = animFile.FileId,
                    Data = bytes
                });

                await _botClient.SendMessage(chatId: msg.Chat.Id, replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true }, text: "✅ Gif was successfully added to the collection.");
            }
            else if (entityValues.Any(ev => ev.Contains("/voteban")))
            {
                var botId = (await _botClient.GetMe()).Id;

                if (msg.ReplyToMessage?.Animation == null
                    || ((msg.ReplyToMessage.From == null || !msg.ReplyToMessage.From.IsBot || msg.ReplyToMessage.From?.Id != botId)
                    && (msg.ReplyToMessage.ViaBot == null || !msg.ReplyToMessage.ViaBot.IsBot || msg.ReplyToMessage.ViaBot.Id != botId)))
                {
                    throw new Exception("Wrong message to create a poll");
                }

                var gifId = int.Parse(msg.ReplyToMessage.Animation!.FileName!.Split('_', '.')[2]);
                var gifType = (GifType)Enum.Parse(typeof(GifType), msg.ReplyToMessage.Animation!.FileName!.Split('_', '.')[1]);

                if (gifType != GifType.Local)
                {
                    throw new Exception("Wrong gif type to create a poll");
                }

                if (!(await _localGifRepo.IsGifExistsAsync(gifId)))
                {
                    await _botClient.SendMessage(chatId: msg.Chat.Id, replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true }, text: "This gif does not exist or has already been removed.");
                    return;
                }

                if (await _pollDataRepository.IsPollDataExists(gifId.ToString(), msg.Chat.Id.ToString()))
                {
                    await _botClient.SendMessage(chatId: msg.Chat.Id, replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true }, text: "A poll for removing this gif already exists.");
                    return;
                }

                var pollKeyboard = InlineKeyboard.CreateVotebanInlineKeyboard(gifId, msg.Chat.Id, 0, 0);

                var gif = await _gifProvider.GetByIdAsync(gifId, GifType.Local);
                using var memory = new MemoryStream(gif.File);

                var file = InputFile.FromStream(memory, $"{gifId}.mp4");

                var pollMsg = await _botClient.SendAnimation(
                    chatId: msg.Chat.Id,
                    animation: file,
                    replyParameters: new ReplyParameters() { MessageId = msg.MessageId, AllowSendingWithoutReply = true },
                    caption: "A voteban poll has been created. Users can vote to remove this gif from the collection.",
                    replyMarkup: pollKeyboard);


                await _pollDataRepository.AddPollDataAsync(new PollData()
                {
                    PollChatId = update.Message!.Chat.Id.ToString(),
                    MessageId = pollMsg.MessageId,
                    GifFileId = gifId,
                    Created = DateTime.UtcNow,
                    Closed = false
                });
            }
        }


    }

}

