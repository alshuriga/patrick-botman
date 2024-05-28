using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatrickBotman.Common.DTO;
using PatrickBotman.Common.Interfaces;

namespace PatrickBotman.AdminPortal.Controllers
{
    [ApiController]
    [Authorize(policy: "UserIsAdmin")]
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetSettings()
        {
            var settings = (await _settingsRepository.GetSettings());
            return Ok(new SettingsDTO()
            {
                AdminID = settings.AdminID,
                MaximumTextLength = settings.MaximumTextLength,
                LocalGifProbability = settings.LocalGifProbability
            });
        }


        [HttpPut("")]
        public async Task<IActionResult> UpdateSettings(UpdateSettingsDTO dto)
        {
            await _settingsRepository.UpdateSettings(new Common.Models.BotSettings()
            {
                AdminID = dto.AdminID,
                MaximumTextLength = dto.MaximumTextLength,
                LocalGifProbability = dto.LocalGifProbability
            });

            return NoContent();
        }
    }
}
