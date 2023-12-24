using Microsoft.AspNetCore.Mvc;
using PatrickBotman.Common.Interfaces;

namespace PatrickBotman.AdminPortal.Controllers
{
    [ApiController]
    [Route("/gifs")]
    public class GifsController : ControllerBase
    {
        private readonly IGifService _gifService;

        public GifsController(IGifService gifService)
        {
            _gifService = gifService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGifsPaginated(int page, long chatId)
        {
            return Ok(await _gifService.GetBlacklistedGifsPageAsync(page, chatId));
        }
    }
}
