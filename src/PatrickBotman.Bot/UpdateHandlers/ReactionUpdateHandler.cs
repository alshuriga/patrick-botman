using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using PatrickBotman.Bot.Helpers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Bot.Services;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using PatrickBotman.Common.Services;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PatrickBotman.Bot.UpdateHandlers
{

    public class ReactionUpdateHandler : IUpdateHandler
    {
        private readonly ILogger<ReactionUpdateHandler> _logger;

        public ReactionUpdateHandler(ILogger<ReactionUpdateHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(Update update)
        {
            _logger.LogInformation($"Recieved reaction {update.MessageReaction.NewReaction[0]} from {update.MessageReaction.User.Username}");
        }
    }
}