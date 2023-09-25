namespace PatrickBotman.Services;

public class FileDownloaderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FileDownloaderService> _logger;
         
    public FileDownloaderService(IHttpClientFactory httpClientFactory, ILogger<FileDownloaderService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<FileInfo> DownloadFile(string url, string path)
    {
        _logger.LogInformation($"Downloading file...");

        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        using (var filestream = File.OpenWrite(path))
        {
            using (var httpstream = await http.GetStreamAsync(url))
            {
                if (httpstream.CanRead || filestream.CanWrite)
                    await httpstream.CopyToAsync(filestream);
            }
        }
        return new FileInfo(path);
    }
}