using PatrickBotman.Bot.Interfaces;
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

        public InlineUpdateHandler(ITelegramBotClient botClient,
            IGifProvider gifService,
            AnimationComposeService animationCompose,
            ILogger<HandleUpdateService> logger)
        {
            _botClient = botClient;
            _gifProvider = gifService;
            _animationCompose = animationCompose;
            _logger = logger;
        }

        public async Task HandleAsync(Update update)
        {
            var inlineQuery = update.InlineQuery!;

            _logger.LogInformation($"Recieved '{inlineQuery.Query}' inline query from user Id '{inlineQuery.From.Id}.'");

            if (String.IsNullOrWhiteSpace(inlineQuery.Query)) return;

            if (!inlineQuery.Query.EndsWith("."))
            {
                await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultArticle(Guid.NewGuid().ToString(), "❌Error",
                    new InputTextMessageContent(inlineQuery.Query)) { Description = "Add a dot ('.') at the end to generate"} });
                return;
            }

            var gifUrl = await _gifProvider.RandomTrendingAsync();

            var file = await _animationCompose.AddText(gifUrl, inlineQuery.Query.Substring(0, inlineQuery.Query.Length - 1));

            if(file.Content != null && file.Content.Length > 0)
            {
                var animationFileId = await UploadAnimationAsync(file);

                await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultCachedMpeg4Gif(Guid.NewGuid().ToString(), animationFileId)
                },
                    isPersonal: true,
                    cacheTime: 5);
            }
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
