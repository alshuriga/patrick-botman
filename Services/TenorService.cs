using PatrickBotman.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatrickBotman.Services;

public class TenorService 
{
    private readonly TenorConfiguration _tenorConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILogger<TenorService> _logger;

    public TenorService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<TenorService> logger)
    {
        _tenorConfiguration = configuration.GetSection("TenorConfiguration").Get<TenorConfiguration>();
        _httpClientFactory = httpClientFactory;

        _logger = logger;
    }

    public async Task<string> RandomTrendingAsync()
    {
        var http = _httpClientFactory.CreateClient("tenorclient");
        http.Timeout = TimeSpan.FromSeconds(10); 
        var url = new Uri($"{_tenorConfiguration.HostAddress}&key={_tenorConfiguration.ApiToken}");
        _logger.LogInformation("Getting gif from Tenor API...");
        var response = await http.GetAsync(url);
        var txt = await response.Content.ReadAsStringAsync();
        var jobject = JObject.Parse(txt);
        _logger.LogInformation("Successfull retrieval of gif URL");
        string[] gif = jobject.SelectToken("results")!.Select(r => r.SelectToken("media_formats.tinymp4.url")!.ToString()).ToArray();
        _logger.LogInformation($"Gifs array length: {gif.Length}");
        return gif[new Random().Next(gif.Length)];

    }

}