using PatrickBotman.Models;
using PatrickBotman.Interfaces;
using Newtonsoft.Json.Linq;

namespace PatrickBotman.Services;

public class GiphyService : IGifService
{
    private readonly GiphyConfiguration _giphyConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GiphyService> _logger;

    public GiphyService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<GiphyService> logger)
    {
        _giphyConfiguration = configuration.GetSection("GiphyConfiguration").Get<GiphyConfiguration>();
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> RandomTrendingAsync()
    {
        var http = _httpClientFactory.CreateClient("Giphyclient");
        var url = new Uri($"{_giphyConfiguration.HostAddress}?api_key={_giphyConfiguration.ApiToken}");

        _logger.LogInformation("Getting gif from Giphy API...");
        
        var response = await http.GetAsync(url);
        var txt = await response.Content.ReadAsStringAsync();
        var jobject = JObject.Parse(txt) ?? throw new Exception("Gif link parsing error");

        _logger.LogInformation("Successfull retrieval of gif URL");

        string gif = jobject.SelectToken("data.images.preview.mp4")!.ToString();
        return gif;
    }

}