using PatrickBotman.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatrickBotman.Services;

public class TenorService 
{
    private readonly TenorConfiguration _tenorConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;

    public TenorService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _tenorConfiguration = configuration.GetSection("TenorConfiguration").Get<TenorConfiguration>();
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> RandomTrendingAsync()
    {
        var http = _httpClientFactory.CreateClient("tenorclient");
        var url = new Uri($"{_tenorConfiguration.HostAddress}&key={_tenorConfiguration.ApiToken}");
        var response = await http.GetAsync(url);
        var txt = await response.Content.ReadAsStringAsync();
        var jobject = JObject.Parse(txt);
        string[] gif = jobject.SelectToken("results")!.Select(r => r.SelectToken("media_formats.tinymp4.url")!.ToString()).ToArray();
        Console.WriteLine(gif.Length);
        return gif[new Random().Next(gif.Length)];

    }

}