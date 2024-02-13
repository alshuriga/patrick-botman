using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PatrickBotman.Bot.UpdateHandlers
{

    public class PollUpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly ILocalGifRepository _gifRepository;
        private readonly IPollDataRepository _pollDataRepository;

        public PollUpdateHandler(ITelegramBotClient botClient,
            ILocalGifRepository gifRepository,
            ILogger<HandleUpdateService> logger,
            IPollDataRepository pollDataRepository)
        {
            _botClient = botClient;
            _logger = logger;
            _gifRepository = gifRepository;
            _pollDataRepository = pollDataRepository;
        }

        public async Task HandleAsync(Update update)
        {
            var poll = update.Poll;
            if (poll == null)
            {
                return;
            }

            var pollData = await _pollDataRepository.GetPollDataAsync(poll.Id);

            if (pollData == null)
            {
                return;
            }

            var chatMembersCount = await _botClient.GetChatMemberCountAsync(pollData.PollChatId);
            var gifFileId = await _gifRepository.GetGifFileId(pollData.GifFileId);

            if(poll.IsClosed && poll.TotalVoterCount < Math.Min(3, Math.Ceiling(chatMembersCount / 2.0)))
            {
                await _botClient.SendAnimationAsync(pollData.PollChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(gifFileId), caption: $"Not enough votes.");
            }
            else if((!poll.IsClosed && poll.TotalVoterCount >= Math.Min(3, Math.Ceiling(chatMembersCount / 2.0))) || poll.IsClosed)
            {
                if(poll.Options[0].VoterCount > poll.Options[1].VoterCount)
                {
                    await _gifRepository.DeleteGifFileAsync(pollData.GifFileId);
                    await _botClient.SendAnimationAsync(pollData.PollChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(gifFileId), caption: $"The gif has been removed.");
                }
            }
              
            if(poll.IsClosed)
            {
                await _pollDataRepository.RemovePollDataAsync(poll.Id);
            }
       }
    }
}
