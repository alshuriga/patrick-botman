using Microsoft.AspNetCore.Mvc;
using PatrickBotman.Services;
using Telegram.Bot.Types;


namespace PatrickBotman.Controllers;

public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
    {
        _logger.LogInformation($"Received update: {update.Type}");
        await handleUpdateService.HandleUpdateAsync(update);
        return Ok();
    }
}