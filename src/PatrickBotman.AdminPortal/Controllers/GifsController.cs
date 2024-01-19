using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using PatrickBotman.Common.Interfaces;
using System.Net.Mime;

namespace PatrickBotman.AdminPortal.Controllers
{
    [Authorize(policy: "UserIsAdmin")]
    [ApiController]
    [Route("/")]
    public class GifsController : ControllerBase
    {
        private readonly IOnlineGifRepository _onlineGifRepo;
        private readonly ILocalGifRepository _localGifRepo;
        private readonly IChatsRepository _chatsRepo;

        public GifsController(IOnlineGifRepository gifService, ILocalGifRepository localGifService, IChatsRepository chatsService)
        {
            _onlineGifRepo = gifService;
            _localGifRepo = localGifService;
            _chatsRepo = chatsService;
        }

        #region GET

        [HttpGet("{chatId}/blacklist")]
        public async Task<IActionResult> GetBlacklistedGifsPaginated(long chatId, int page)
        {
            return Ok(await _onlineGifRepo.GetBlacklistedGifsPageAsync(page, chatId));
        }

        [HttpGet("local")]
        public async Task<IActionResult> GetLocalGifsPaginated(int page)
        {
            return Ok(await _localGifRepo.GetGifFilesPageAsync(page));
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsPaginated(int page)
        {
            return Ok(await _chatsRepo.GetChatsPageAsync(page));
        }  

        [HttpGet("file")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadGif(int id)
        {
            var file = await _localGifRepo.GetGifFileAsync(id);

            var contentType = file.Name.EndsWith("mp4") ? "video/mp4" : "image/gif";

            return File(file.Data, contentType, file.Name);
        }

        #endregion

        #region POST

        [HttpPost("file")]
        public async Task<IActionResult> UploadGif(IFormFile file)
        {
            using var strm = file.OpenReadStream();
            var msyt = new MemoryStream();
            strm.CopyTo(msyt);
            var data = msyt.ToArray();

            await _localGifRepo.CreateGifFileAsync(new Common.Persistence.Entities.GifFile()
            {
                Data = data, 
                Name = file.Name,
            });

            return NoContent();
        }

        #endregion

        #region PUT

        #endregion

        #region DELETE

        [HttpDelete("local")]
        public async Task<IActionResult> DeleteLocalGif(int id)
        {
            await _localGifRepo.DeleteGifFileAsync(id);
            return NoContent();
        }

        #endregion

    }
}
