using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using PatrickBotman.Common.Interfaces;
using System.Net.Mime;

namespace PatrickBotman.AdminPortal.Controllers
{
    [ApiController]
    [Route("/")]
    public class GifsController : ControllerBase
    {
        private readonly IGifService _gifService;

        public GifsController(IGifService gifService)
        {
            _gifService = gifService;
        }

        [HttpGet("{chatId}/gif")]
        public async Task<IActionResult> GetGifsPaginated(long chatId, int page)
        {
            return Ok(await _gifService.GetBlacklistedGifsPageAsync(page, chatId));
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsPaginated(int page)
        {
            return Ok(await _gifService.GetChatsPageAsync(page));
        }

        [HttpPost("file")]
        public async Task<IActionResult> UploadGif(IFormFile file)
        {
            using var strm = file.OpenReadStream();
            var msyt = new MemoryStream();
            strm.CopyTo(msyt);
            var data = msyt.ToArray();

            await _gifService.AddNewGifFileAsync(new Common.DTO.GifFileDTO(file.Name, data));

            return NoContent();
        }

        [HttpGet("file")]
        public async Task<IActionResult> DownloadGif(int id)
        {
            var file = await _gifService.GetGifFileAsync(id);

            var contentType = file.fileName.EndsWith("mp4") ? "video/mp4" : "image/gif";

            return File(file.data, contentType, file.fileName);
        }

    }
}
