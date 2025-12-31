using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.Helpers;
using PatrickBotman.Services;
using Telegram.Bot.Types;

namespace PatrickBotman.Controllers;

public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly BotConfiguration _botConfig;

    public WebhookController(ILogger<WebhookController> logger, IOptionsSnapshot<BotConfiguration> botConfiguration)
    {
        _logger = logger;
        _botConfig = botConfiguration.Value;

    }
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
    {
        _logger.LogInformation($"Received update: {update.Type}");
        await handleUpdateService.HandleUpdateAsync(update);
        return Ok();
    }
}