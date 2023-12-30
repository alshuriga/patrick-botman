using Newtonsoft.Json.Linq;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.Interfaces;

namespace PatrickBotman.Services;

public class GIfProvider : IGifProvider
{
    private readonly GiphyConfiguration _giphyConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GIfProvider> _logger;
    private readonly IGifService _gifService;

    public GIfProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<GIfProvider> logger, IGifService gifService)
    {
        _giphyConfiguration = configuration.GetSection("GiphyConfiguration").Get<GiphyConfiguration>();
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _gifService = gifService;
    }

    public async Task<Gif> RandomGifAsync(long chatId)
    {
        return await RandomOnlineAsync(chatId);
    }


    //random giphy gif (not blacklisted in db for specified chat id)
    private async Task<Gif> RandomOnlineAsync(long chatId)
    {
        var http = _httpClientFactory.CreateClient("Giphyclient");
        http.Timeout = TimeSpan.FromSeconds(5);

        var url = new Uri($"{_giphyConfiguration.HostAddress}?api_key={_giphyConfiguration.ApiToken}");

        string gifUrl;

        do
        {
            _logger.LogInformation("Getting gif from Giphy API...");

            var response = await http.GetAsync(url);
            var txt = await response.Content.ReadAsStringAsync();
            var jobject = JObject.Parse(txt) ?? throw new Exception("Gif link parsing error");

            _logger.LogInformation("Successfull retrieval of gif URL");

            gifUrl = jobject.SelectToken("data.images.preview.mp4")!.ToString();
        }
        while ((await _gifService.IsBlacklistedAsync(gifUrl, chatId)));

        var id = await _gifService.GetIdOrCreateAsync(gifUrl);

        var gifBytes = await http.GetByteArrayAsync(gifUrl);

        return new Gif()
        {
            Id = id,
            File = gifBytes,
            Type = GifType.Online
        };
    }

}