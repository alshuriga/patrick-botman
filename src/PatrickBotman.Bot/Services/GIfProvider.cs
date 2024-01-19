using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.DTO;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence.Entities;
using static System.Net.WebRequestMethods;

namespace PatrickBotman.Services;

public class GIfProvider : IGifProvider
{
    private readonly GiphyConfiguration _giphyConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GIfProvider> _logger;
    private readonly IOnlineGifRepository _onlineRepo;
    private readonly ILocalGifRepository _localRepo;
    private readonly BotConfiguration _botConfiguration;

    public GIfProvider(IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GIfProvider> logger,
        IOnlineGifRepository gifService,
        ILocalGifRepository localRepo,
        IOptionsSnapshot<BotConfiguration> botConfiguration)
    {
        _giphyConfiguration = configuration.GetSection("GiphyConfiguration").Get<GiphyConfiguration>();
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _onlineRepo = gifService;
        _localRepo = localRepo;
        _botConfiguration = botConfiguration.Value;
    }

    public async Task<GifFileWithType> RandomGifAsync(long chatId)
    {
        var random = new Random().Next(0, 100);

        _logger.LogDebug($"random num: {random}, probability: {_botConfiguration.LocalGifsProbability}");

        var isLocal = (new Random().Next(0, 100) <= _botConfiguration.LocalGifsProbability);

        if(isLocal)
        {
            _logger.LogInformation("Using gif from local collection");
            return await RandomLocalAsync();
        }
        else
        {
            _logger.LogInformation("Using gif from online");
            return await RandomOnlineAsync(chatId);
        }
    }


    //random giphy gif (not blacklisted in db for specified chat id)
    private async Task<GifFileWithType> RandomOnlineAsync(long chatId, bool urlOnly = false)
    {
        var http = _httpClientFactory.CreateClient("Giphyclient");

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
        while ((await _onlineRepo.IsBlacklistedAsync(gifUrl, chatId)));

        var id = await _onlineRepo.GetIdOrCreateAsync(gifUrl);

        var gifBytes = urlOnly ? null! : await http.GetByteArrayAsync(gifUrl);

        return new GifFileWithType()
        {
            Id = id,
            File = gifBytes,
            Type = GifType.Online,
            Link = gifUrl,
        };
    }

    private async Task<GifFileWithType> RandomLocalAsync()
    {
        var gif = await _localRepo.GetRandomGifFileAsync();

        return new GifFileWithType()
        {
            Id = gif.Id,
            File = gif.Data,
            Type = GifType.Local
        };
    }

    public async Task<IEnumerable<GifFileWithType>> RandomPreviewsAsync(int count)
    {
        //currently provides local gifs only
        var res = (await _localRepo.GetRandomGifFilesAsync(count))
            .Select((GifFile gif) => new GifFileWithType()
            {
                Id = gif.Id,
                File = gif.Data,
                Type = GifType.Local,
                Link = gif.Name
            });

        //var gif = await RandomOnlineAsync(0, urlOnly: true);

        //res = res.Append(gif);

        return res;
    }

    public async Task<GifFileWithType> GetByIdAsync(int id, GifType type)
    {
        if (type == GifType.Online)
        {
            var gif = await _onlineRepo.GetByIdAsync(id);
            var http = _httpClientFactory.CreateClient("Giphyclient");
            var gifBytes = await http.GetByteArrayAsync(gif.GifUrl);

            return new GifFileWithType()
            {
                Id = id,
                File = gifBytes,
                Type = GifType.Online
            };
        }
        else
        {
            var gif = await _localRepo.GetGifFileAsync(id);
            return new GifFileWithType()
            {
                Id = id,
                File = gif.Data,
                Type = GifType.Local
            };
        }
    }
}