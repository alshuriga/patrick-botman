using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.InlineQueryResults;
using PatrickBotman.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.VisualBasic;
using System.Collections;
using PatrickBotman.Persistence.Entities;
using PatrickBotman.Models;
namespace PatrickBotman.Services;

public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IGifService _gifService;
    private readonly AnimationEditService _edit;
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly int _maximumTextLength;
    private readonly IGifRatingRepository _gifRatingRepository;


    public HandleUpdateService(ITelegramBotClient botClient, IGifService gifService, AnimationEditService edit, ILogger<HandleUpdateService> logger, IConfiguration configuration, IGifRatingRepository gifRatingRepository)
    {
        _gifRatingRepository = gifRatingRepository;
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
                UpdateType.CallbackQuery => HandleCallbackAsync(update.CallbackQuery!),
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
            if (!messageText.Contains($"/gif")) return;
            messageText = msg.ReplyToMessage?.Caption ?? msg.ReplyToMessage?.Text;
            if (messageText == null) return;
        }

        var isFavoriteGif = new Random().Next(0, 100) > 30;

        GifDTO? gifDTO = null;

        if(isFavoriteGif)
        {
            gifDTO = await _gifRatingRepository.GetRandomGifAsync(msg.Chat.Id);
        }
        if(!isFavoriteGif || gifDTO == null)
        {
            var url = await _gifService.RandomTrendingAsync();
            var id = await _gifRatingRepository.GetGifIdAsync(url);
            gifDTO = new GifDTO(id, url);
        }

        var rating = await _gifRatingRepository.GetGifRatingAsync(gifDTO.GifId, msg.Chat.Id);

        var file = await _edit.AddText(gifDTO.GifUrl, messageText);

        if (file != null) {
            await using (var stream = System.IO.File.OpenRead(file))
            {
                stream.Position = 0;
                await _botClient.SendAnimationAsync(
                            replyMarkup: CreateVotingInlineKeyboard(rating, gifDTO.GifId),
                            chatId: msg.Chat.Id,
                            animation: new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4"));
            }
        }

    }

    private async Task InlineQueryRespondAsync(InlineQuery inlineQuery)
    {
        _logger.LogInformation($"Recieved '{inlineQuery.Query}' inline query from user Id '{inlineQuery.From.Id}.'");

        if (String.IsNullOrWhiteSpace(inlineQuery.Query)) return;

            if (!inlineQuery.Query.EndsWith(".")) {
            await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultArticle(Guid.NewGuid().ToString(), "❌Error",
                    new InputTextMessageContent(inlineQuery.Query)) { Description = "Add a dot ('.') at the end to generate"} });
                return;
            }

            var gifUrl = await _gifService.RandomTrendingAsync();
            var file = await _edit.AddText(gifUrl, inlineQuery.Query.Substring(0, inlineQuery.Query.Length - 1));

            if (file != null)
                await using (var stream = System.IO.File.OpenRead(file))
                {
                    InputOnlineFile? tgFile = new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4");
                    var animationFileId = await UploadAnimationAsync(tgFile);
                    await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, new InlineQueryResult[] {
                    new InlineQueryResultCachedMpeg4Gif(Guid.NewGuid().ToString(), animationFileId) }, isPersonal: true, cacheTime: 5);
                };

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


    private async Task HandleCallbackAsync(CallbackQuery callbackQuery)
    {
        if(callbackQuery.Data == null || callbackQuery.Data == "ignore") return;

        if(callbackQuery.Message?.MessageId != null && callbackQuery.Message?.Chat.Id != null)
        {
            var userId = callbackQuery.From.Id;
            var chatId = callbackQuery.Message.Chat.Id;

            if(callbackQuery.Data.StartsWith("up"))
            {
                var gifId = int.Parse(callbackQuery.Data.Split(' ')[1]);
                await _gifRatingRepository.UpvoteGifAsync(gifId, userId, chatId);
            }
            else if(callbackQuery.Data.StartsWith("down"))
            {
                var gifId = int.Parse(callbackQuery.Data.Split(' ')[1]);
                await _gifRatingRepository.DownvoteGifAsync(gifId, userId, chatId);
            }

            var rating = await _gifRatingRepository.GetGifRatingAsync(int.Parse(callbackQuery.Data.Split(' ')[1]), chatId);
            
            await _botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, CreateVotingInlineKeyboard(rating, int.Parse(callbackQuery.Data.Split(' ')[1])));    
        }
         
    }


    private InlineKeyboardMarkup CreateVotingInlineKeyboard(int rating, int gifId)
    {
        IEnumerable<InlineKeyboardButton> buttons = new[] { InlineKeyboardButton.WithCallbackData($"👎",  $"down {gifId}"),
            InlineKeyboardButton.WithCallbackData($"{rating}", "ignore"),
            InlineKeyboardButton.WithCallbackData($"👍", $"up {gifId}")};

        var replyMarkup = new InlineKeyboardMarkup(buttons);

        return replyMarkup;
    }
}