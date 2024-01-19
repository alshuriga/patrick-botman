using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;

namespace PatrickBotman.Bot.UpdateHandlers
{
    public class InlineUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IGifProvider _gifProvider;
        private readonly AnimationComposeService _animationCompose;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly BotConfiguration _botConfig;

        public InlineUpdateHandler(ITelegramBotClient botClient,
            IGifProvider gifService,
            AnimationComposeService animationCompose,
            ILogger<HandleUpdateService> logger,
            IOptions<BotConfiguration> botConfig)
        {
            _botClient = botClient;
            _gifProvider = gifService;
            _animationCompose = animationCompose;
            _logger = logger;
            _botConfig = botConfig.Value;
        }

        public async Task HandleAsync(Update update)
        {
            var inlineQuery = update.InlineQuery!;

            _logger.LogInformation($"Recieved '{inlineQuery.Query}' inline query from user Id '{inlineQuery.From.Id}.'");

            if (string.IsNullOrWhiteSpace(inlineQuery.Query)) return;

            //if (!inlineQuery.Query.EndsWith("."))
            //{
            //    await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
            //        new InlineQueryResultArticle(Guid.NewGuid().ToString(), "❌Error",
            //        new InputTextMessageContent(inlineQuery.Query)) { Description = "Add a dot ('.') at the end to generate"} });
            //    return;
            //}

            //var gif = await _gifProvider.RandomGifAsync(inlineQuery.From.Id);
            //var file = await _animationCompose.ComposeGifAsync(gif, inlineQuery.Query.Substring(0, inlineQuery.Query.Length - 1));

            //if (file.Content != null && file.Content.Length > 0)
            //{
            //    var animationFileId = await UploadAnimationAsync(file);

            //    await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
            //        new InlineQueryResultCachedMpeg4Gif(Guid.NewGuid().ToString(), animationFileId)
            //    },
            //        isPersonal: true,
            //        cacheTime: 5);
            //}

            var gifFiles = await _gifProvider.RandomPreviewsAsync(3);
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton("LOADING") { CallbackData = "." });
            var inlineResults = await Task.WhenAll(gifFiles!.Select(async g =>
            {
                string id = null!;


                if (g.Type == GifType.Local)
                {
                    id = g.Link!;
                }
                else
                {
                    id = await UploadAnimationAsync(new InputOnlineFile(g.Link!));
                }

                return new InlineQueryResultCachedMpeg4Gif($"{g.Type} {g.Id}", id)
                {
                    ReplyMarkup = keyboard
                };

            }));
                await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, inlineResults,
                isPersonal: false,
                cacheTime: 10); ;
        }

        private async Task<string> UploadAnimationAsync(InputOnlineFile file)
        {
            _logger.LogInformation("Animation uploading...");
            var msg = await _botClient.SendAnimationAsync(
                                chatId: 35306756,
                                animation: file,
                                disableNotification: true
                            );

            var fileId = msg.Animation!.FileId;
            await _botClient.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
            return msg.Animation.FileId;

        }
    }
}
