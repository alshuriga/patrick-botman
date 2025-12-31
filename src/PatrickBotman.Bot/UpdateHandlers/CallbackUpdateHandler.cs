using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PatrickBotman.Bot.UpdateHandlers
{

    public class CallbackUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IOnlineGifRepository _gifRepository;

        private readonly ILocalGifRepository _localGifRepository;
        private readonly IPollDataRepository _pollDataRepository;


        public CallbackUpdateHandler(ITelegramBotClient botClient,
            IOnlineGifRepository gifRepository,
            IPollDataRepository pollDataRepository,  
            ILocalGifRepository localGifRepository,
            ILogger<HandleUpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
            _gifRepository = gifRepository;
            _pollDataRepository = pollDataRepository;
            _localGifRepository = localGifRepository;
        }

        public async Task HandleAsync(Update update)
        {
            var callbackQuery = update.CallbackQuery!;

            if (callbackQuery.Message?.MessageId == null || callbackQuery.Message?.Chat.Id == null || callbackQuery.Data == null) return;

            var chatId = callbackQuery.Message.Chat.Id;

            if (callbackQuery.Data.StartsWith("blacklist"))
            {
                var gifId = int.Parse(callbackQuery.Data.Split(' ')[1]);

                await _gifRepository.BlacklistAsync(gifId, chatId);

                await _botClient.AnswerCallbackQuery(callbackQuery.Id, $"The GIF has been blacklisted 🚮", showAlert: false ,cacheTime: 10);
              
                await _botClient.EditMessageReplyMarkup(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(Enumerable.Empty<InlineKeyboardButton>()));
                
                var whoBlacklisted = callbackQuery.From.Username != null ? $"@{callbackQuery.From.Username}" : callbackQuery.From.FirstName;
                await _botClient.EditMessageCaption(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"🚮 blacklisted by {whoBlacklisted}");
            }
            else if(callbackQuery.Data.StartsWith("voteban"))
            {
                var parts = callbackQuery.Data.Split(':');
                int voteType = parts[1] == "-" ? -1 : 1;
                var gifId = parts[2];
                var pollChatId = parts[3];

                var pollData = await _pollDataRepository.AddVoteToPollAsync(gifId, pollChatId, callbackQuery.From.Id.ToString(), voteType);

                var chatMembersCount = await _botClient.GetChatMemberCount(pollChatId);

                var forCount = pollData.PollVote.Where(v => v.Vote > 0).Count();
                var againstCount = pollData.PollVote.Where(v =>  v.Vote < 0).Count();

                if(forCount >= Math.Floor((chatMembersCount - 2) / 2.0) + 1)
                {
                    await _localGifRepository.DeleteGifFileAsync(pollData.GifFileId);
                    await _botClient.EditMessageReplyMarkup(long.Parse(pollChatId), callbackQuery.Message.MessageId, null);
                    await _botClient.EditMessageText(long.Parse(pollChatId), callbackQuery.Message.MessageId, "Poll is closed. Gif has been banned");
                    return;
                }

                if (againstCount >= Math.Floor((chatMembersCount - 2) / 2.0) + 1)
                {
                    await _botClient.EditMessageReplyMarkup(long.Parse(pollChatId), callbackQuery.Message.MessageId, null);
                    await _botClient.EditMessageText(long.Parse(pollChatId), callbackQuery.Message.MessageId, "Poll is closed. Gif has not been banned");
                    return;
                }

                else
                {
                   var updatedKeyboard = InlineKeyboard.CreateVotebanInlineKeyboard(pollData.GifFileId, long.Parse(pollChatId), forCount, againstCount);
                   await _botClient.EditMessageReplyMarkup(long.Parse(pollChatId), callbackQuery.Message.MessageId, updatedKeyboard);
                }

                await _botClient.AnswerCallbackQuery(callbackQuery.Id, "Your vote has been recorded.", showAlert: false, cacheTime: 10);

            }
        }
    }
}
