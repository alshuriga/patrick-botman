﻿using patrick_botman.Helpers;
using PatrickBotman.Interfaces;
using PatrickBotman.Models;
using PatrickBotman.Models.DTOs;
using PatrickBotman.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace patrick_botman.UpdateHandlers
{

    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<MessageUpdateHandler> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly AnimationEditService _edit;
        private readonly IGifRepository _gifRepository;
        private readonly IGifService _gifService;


        public MessageUpdateHandler(ILogger<MessageUpdateHandler> logger,
            ITelegramBotClient botClient,
            AnimationEditService edit,
            IGifRepository gifRepository,
            IGifService gifService)
        {
            _botClient = botClient;
            _edit = edit;
            _gifRepository = gifRepository;
            _gifService = gifService;
            _logger = logger;
        }
        public async Task HandleAsync(Update update)
        {
            var msg = update.Message!;

            _logger.LogInformation($"Recieved '{msg.Text}' message from user Id '{msg.From?.Id}.'");
            string? messageText = msg.Caption ?? msg.Text;

            if (messageText == null) return;

            if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
            {
                if (!messageText.Contains($"/gif")) return;
                messageText = msg.ReplyToMessage?.Caption ?? msg.ReplyToMessage?.Text;
                if (messageText == null) return;
            }

            string url;

            while (true)
            {
                url = await _gifService.RandomTrendingAsync();
                var isBlacklisted = await _gifRepository.IsBlacklistedAsync(url, msg.Chat.Id);
                if (!isBlacklisted) break;
            }

            var gifId = await _gifRepository.GetIdOrCreateAsync(url);

            var file = await _edit.AddText(url, messageText);

            if (file != null)
            {
                await using (var stream = System.IO.File.OpenRead(file))
                {
                    stream.Position = 0;
                    await _botClient.SendAnimationAsync(
                                replyMarkup: InlineKeyboard.CreateVotingInlineKeyboard(gifId),
                                chatId: msg.Chat.Id,
                                animation: new InputOnlineFile(stream, Guid.NewGuid().ToString() + ".mp4"));
                }
            }

        }

    }
}
