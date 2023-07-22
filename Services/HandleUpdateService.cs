using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.InlineQueryResults;
using PatrickBotman.Interfaces;

namespace PatrickBotman.Services;

public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IGifService _gifService;
    private readonly AnimationEditService _edit;
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly int _maximumTextLength;


    public HandleUpdateService(ITelegramBotClient botClient, IGifService gifService, AnimationEditService edit, ILogger<HandleUpdateService> logger, IConfiguration configuration)
    {
        _botClient = botClient;
        _gifService = gifService;
        _edit = edit;
        _logger = logger;
        _maximumTextLength = configuration.GetValue<int>("MaximumTextLength");
    }

    public async Task HandleUpdateAsync(Update update)
    {
        try
        {
            await (update.Type switch
            {
                UpdateType.Message => EchoAsync(update.Message!),
                UpdateType.InlineQuery => InlineQueryRespondAsync(update.InlineQuery!),
                _ => Task.CompletedTask
            });
        }
        finally
        {

           await _edit.Clean();
        }

    }

    private async Task EchoAsync(Message msg)
    {
        _logger.LogInformation($"Recieved '{msg.Text}' message from user Id '{msg.From?.Id}.'");
        string? messageText = msg.Caption ?? msg.Text;
        if (messageText == null) return;
        var botUserName = (await _botClient.GetMeAsync()).Username;

        if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
        {
            if (!(messageText.Contains($"/gif"))) return;
            messageText = msg.ReplyToMessage?.Caption ?? msg.ReplyToMessage?.Text;
            if (messageText == null) return;
        }
        var gifUrl = await _gifService.RandomTrendingAsync();
        var file = await _edit.AddText(gifUrl, messageText);

        if (file != null) {
            await using (var stream = System.IO.File.OpenRead(file))
            {
                stream.Position = 0;
                await _botClient.SendAnimationAsync(
                            chatId: msg.Chat.Id,
                            animation: new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4")
                        );
            }
        }

    }

    private async Task InlineQueryRespondAsync(InlineQuery inlineQuery)
    {
        _logger.LogInformation($"Recieved '{inlineQuery.Query}' inline query from user Id '{inlineQuery.From.Id}.'");

        if (String.IsNullOrWhiteSpace(inlineQuery.Query)) return;

        try
        {
            if (!inlineQuery.Query.EndsWith(".")) throw new FormatException("Add a dot ('.') at the end to generate");
            var gifUrl = await _gifService.RandomTrendingAsync();
            var file = await _edit.AddText(gifUrl, inlineQuery.Query.Substring(0, inlineQuery.Query.Length - 1));
            InputOnlineFile? tgFile = null;

            if (file != null)
                await using (var stream = System.IO.File.OpenRead(file))
                {
                    tgFile = new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4");
                    var animationFileId = await UploadAnimationAsync(tgFile);
                    await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultCachedMpeg4Gif(Guid.NewGuid().ToString(), animationFileId) }, isPersonal: true, cacheTime: 5);
                };
        }

        catch (FormatException ex)
        {
            await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultArticle(Guid.NewGuid().ToString(), "❌Error", new InputTextMessageContent(ex.Message)) {Description = ex.Message}
            });
        }
    }


    private async Task ReportErrorAsync(Exception ex, Update update)
    {
        _logger.LogError($"{ex.Message}:{ex.Source}");
        if (update.Message is { } message)
            await _botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: $"❌ {ex.Message}\n\n{ex.StackTrace}\n\n{ex.Source}"
                            );
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