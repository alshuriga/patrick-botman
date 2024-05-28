using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace PatrickBotman.Bot.UpdateHandlers
{
    public class ChosenInlineUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IGifProvider _gifProvider;
        private readonly AnimationComposeService _animationCompose;
        private readonly ILogger<HandleUpdateService> _logger;

        public ChosenInlineUpdateHandler(ITelegramBotClient botClient,
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
            var chosenInline = update.ChosenInlineResult;

            if(chosenInline?.InlineMessageId == null && chosenInline?.From == null)
            {
                throw new ArgumentException(nameof(chosenInline));
            }

            _logger.LogInformation($"User {chosenInline.From} chose inline result.");

            var gifType = (GifType)Enum.Parse(typeof(GifType), chosenInline.ResultId.Split(' ')[0]);
            var gifId = int.Parse(chosenInline.ResultId.Split(' ')[1]);

            var gif = await _gifProvider.GetByIdAsync(gifId, gifType);

            var file = await _animationCompose.ComposeGifAsync(gif, chosenInline.Query);

            if(file.Content != null && file.Content.Length > 0)
            {
                var animationFileId = await UploadAnimationAsync(file);
                await _botClient.EditMessageMediaAsync(chosenInline.InlineMessageId!, new InputMediaAnimation(animationFileId));
               
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
