using Microsoft.AspNetCore.Mvc;
using PatrickBotman.Services;
using Telegram.Bot.Types;


namespace PatrickBotman.Controllers;

public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
    {
        await handleUpdateService.HandleUpdateAsync(update);
        return Ok();
    }

    [HttpGet("/test")]
    public ContentResult Test()
    {
        return Content("Test");
    }
}