
using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PatrickBotman.Bot.Services
{
    public class PollProcessingService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PollProcessingService> _logger;

        public PollProcessingService(ILogger<PollProcessingService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"PollProcessingService running at: {DateTimeOffset.Now}");
                await DoWork();
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            using var scope = _services.CreateScope();
            var _pollDataRepository = scope.ServiceProvider.GetRequiredService<IPollDataRepository>();
            var _botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var _localGifRepository = scope.ServiceProvider.GetRequiredService<ILocalGifRepository>();
            var _botConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<BotConfiguration>>().Value;

            _logger.LogInformation($"Poll processing: {DateTime.Now}");

            var openedPolls = await _pollDataRepository.GetOpenPollsAsync();

            var timedOutPolls = openedPolls.Where(p => DateTime.UtcNow.Subtract(p.Created) >= TimeSpan.FromSeconds(_botConfig.PollLifetime)).ToList();

            _logger.LogInformation($"Closing {timedOutPolls.Count} outdated polls");

            await _pollDataRepository.ClosePollsByIds(timedOutPolls.Select(p => p.Id));

            foreach (var pollData in timedOutPolls)
            {
                var forCount = pollData.PollVote.Where(v => v.Vote > 0).Count();
                var againstCount = pollData.PollVote.Where(v => v.Vote < 0).Count();
                var chatMembersCount = await _botClient.GetChatMemberCount(pollData.PollChatId);
                var pollChatId = long.Parse(pollData.PollChatId);
                var messageId = pollData.MessageId;

                var replyParameters = new ReplyParameters()
                {
                    MessageId = messageId,
                    ChatId = pollChatId,
                    AllowSendingWithoutReply = true
                };

                if (forCount - againstCount > 0)
                {
                    await _localGifRepository.DeleteGifFileAsync(pollData.GifFileId);
                    var msg = "Poll is closed. Gif has been banned";
                    await _botClient.EditMessageCaption(pollChatId, messageId, msg);
                    await _botClient.SendMessage(chatId: pollChatId, text: msg, replyParameters: replyParameters);

                    return;
                }
                else
                {
                    await _botClient.EditMessageReplyMarkup(pollChatId, messageId, null);
                    var msg = "Poll is closed. Gif has not been banned";
                    await _botClient.EditMessageCaption(pollChatId, messageId, msg);
                    await _botClient.SendMessage(chatId: pollChatId, text: msg, replyParameters: replyParameters);
                    return;
                }
            }
        }
    }
}
