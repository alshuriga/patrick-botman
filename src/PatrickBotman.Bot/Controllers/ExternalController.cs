using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatrickBotman.Bot.Models;
using PatrickBotman.Services;
using Telegram.Bot.Types;


namespace PatrickBotman.Controllers;

[Authorize(AuthenticationSchemes = HeaderAuthHandler.SchemeName)]
public class ExternalController : ControllerBase
{
    private readonly ILogger<ExternalController> _logger;

    public ExternalController(ILogger<ExternalController> logger)
    {
        _logger = logger;
    }

    [HttpPost("quote")]
    public async Task<IActionResult> Quote([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
    {
        _logger.LogInformation($"Received external update: {update.Type}");
        await handleUpdateService.HandleUpdateAsync(update);
        return Ok();
    }
}