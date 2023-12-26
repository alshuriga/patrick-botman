using System.Diagnostics;
using System.Runtime.InteropServices;
using PatrickBotman.Bot.Models;
using Telegram.Bot.Types.InputFiles;

namespace PatrickBotman.Bot.Services;

public class AnimationComposeService
{
    private readonly string _ffmpegBinary;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AnimationComposeService> _logger;


    public AnimationComposeService(IConfiguration configuration,
        ILogger<AnimationComposeService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;

        _ffmpegBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
           ? "ffmpeg"
           : "ffmpeg.exe";

        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<InputOnlineFile> AddText(string url, string text)
    {
        //set ffmpeg arguments
        var textInput = new TextInput(text, _configuration);
        var maxLineLength = Math.Max(textInput.FirstLine.Length, textInput.SecondLine.Length);
        string argsTemplate = "drawtext=fontsize=min(((w*0.98)/20)*2\\,((w*0.98)/{0})*2):line_spacing=4:font='Impact':text='{1}':fix_bounds=true:x=(w-text_w)/2:y=(h*{2}-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
        string firstLineArgs = string.Format(argsTemplate, maxLineLength, textInput.FirstLine, 0.1);
        string secondLineArgs = string.Format(argsTemplate, maxLineLength, textInput.SecondLine, 0.9);

        var outputStream = new MemoryStream();

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"-i pipe: -f gif -vf \"scale=480:-1,{string.Join(',', new string[] { firstLineArgs, secondLineArgs })}\" pipe:",
            FileName = _ffmpegBinary
        };

        using Process process = new Process()
        {
            StartInfo = processInfo,
            EnableRaisingEvents = true
        };

        process.ErrorDataReceived += (e, d) => _logger.LogError(d.Data);
        process.OutputDataReceived += (e, d) =>  _logger.LogInformation(d.Data);

        process.Start();

        await using var inputStream = await GetFileStreamAsync(url);

        await inputStream.CopyToAsync(process.StandardInput.BaseStream);

        process.StandardInput.Close();

        await process.StandardOutput.BaseStream.CopyToAsync(outputStream);
        await process.WaitForExitAsync();

        outputStream.Position = 0;

        return new InputOnlineFile(outputStream, $"{Guid.NewGuid()}.gif");
    }



    private Task<Stream> GetFileStreamAsync(string url)
    {
        //download file to stream
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        return http.GetStreamAsync(url);
    }
}