using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
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
        private readonly IGifService _gifService;
        private readonly IGifProvider _gifProvider;
        private readonly IPollDataRepository _pollDataRepository;
        private readonly BotConfiguration _options;


        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationComposeService edit,
            IGifService gifService,
            IGifProvider gifProvider,
<<<<<<< Updated upstream
            IOptions<BotConfiguration> options
            )
=======
            IOptionsSnapshot<BotConfiguration> options,
            ILocalGifRepository localGifRepo,
            IPollDataRepository pollDataRepository)
>>>>>>> Stashed changes
        {
            _botClient = botClient;
            _edit = edit;
            _gifService = gifService;
            _gifProvider = gifProvider;
            _logger = logger;
            _options = options.Value;
<<<<<<< Updated upstream
=======
            _localGifRepo = localGifRepo;
            _pollDataRepository = pollDataRepository;
>>>>>>> Stashed changes
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message!;

            _logger.LogInformation($"Recieved a message from user {msg.From?.Id}");

            var entityValues = msg.EntityValues ?? Enumerable.Empty<string>();

            if (entityValues.Any(ev => ev.Contains("/gif")) || (msg.Chat.Type == ChatType.Private
                && !entityValues.Any(ev => ev.Contains("/add"))
                 && !entityValues.Any(ev => ev.Contains("/voteban"))))
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
                animation: tgFile,
                replyToMessageId: msg.MessageId);

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
                else if (await _localGifRepo.IsGifExistsAsync(msg.ReplyToMessage.Animation.FileId))
                {
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id,
                        replyToMessageId: msg.MessageId,
                        allowSendingWithoutReply: true,
                        text: "🚫 The gif is already in the collection");

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
            else if (entityValues.Any(ev => ev.Contains("/voteban")))
            {
                if (msg.ReplyToMessage?.Animation == null
                    || msg.ReplyToMessage.From == null
                    || !msg.ReplyToMessage.From.IsBot
                    || msg.ReplyToMessage.From.Id != (await _botClient.GetMeAsync()).Id)
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
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id, replyToMessageId: msg.MessageId, allowSendingWithoutReply: true, text: "This gif does not exist or has already been removed.");
                    return;
                }

                if (await _pollDataRepository.IsPollDataExists(gifId))
                {
                    await _botClient.SendTextMessageAsync(chatId: msg.Chat.Id, replyToMessageId: msg.MessageId, allowSendingWithoutReply: true, text: "A poll for removing this gif already exists.");
                    return;
                }


                var pollMsg = await _botClient.SendPollAsync(chatId: msg.Chat.Id,
                    replyToMessageId: msg.MessageId,
                    allowSendingWithoutReply: true,
                    isAnonymous: true,
                    type: PollType.Regular,
                    explanationParseMode: ParseMode.Markdown,
                    question: $"Do you want the gif to be removed?",
                    options: new[] { "👍", "👎" });

                await _pollDataRepository.AddPollDataAsync(new PollData()
                {
                    PollChatId = pollMsg.Chat.Id,
                    PollId = pollMsg.Poll!.Id,
                    GifFileId = gifId,
                });



#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await Task.Delay((_options.PollLifetime * 1000) - 100);

                    var poll = await _botClient.StopPollAsync(pollMsg.Chat.Id, pollMsg.MessageId);
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed



            }

        }


    }

}

