using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using FFmpeg.NET.Enums;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.Helpers;
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

    public async Task<InputOnlineFile> ComposeGifAsync(Gif gif, string text)
    {
        var isNewYear = DateTime.Now >= new DateTime(day: 23, month: 12, year: DateTime.Now.Year) || DateTime.Now < new DateTime(day: 7, month: 1, year: DateTime.Now.Year);

        var workDir = _configuration.GetValue<string>("AssetsDirectory")!;

        //set ffmpeg arguments
        var textInput = new TextInput(text, _configuration);
        var maxLineLength = Math.Max(textInput.FirstLine.Length, textInput.SecondLine.Length);
        string argsTemplate = "drawtext=fontsize=min(((w*0.98)/20)*2\\,((w*0.98)/{0})*2):line_spacing=4:fontfile='assets/impact.ttf':text='{1}':fix_bounds=true:x=(w-text_w)/2:y=(h*{2}-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
        string firstLineArgs = string.Format(argsTemplate, maxLineLength, textInput.FirstLine, 0.1);
        string secondLineArgs = string.Format(argsTemplate, maxLineLength, textInput.SecondLine, 0.9);

        var outputStream = new MemoryStream();

        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = isNewYear ? $"-i pipe: -i assets/snow.mov -f gif  -filter_complex \"scale=350:-1,pad=ceil(iw/2)*2:ceil(ih/2)*2,overlay=shortest=1,{string.Join(',', new string[] { firstLineArgs, secondLineArgs })}\" pipe:"
                : $"-i pipe: -f gif -vf \"scale=300:-1,{string.Join(',', new string[] { firstLineArgs, secondLineArgs })}\" pipe:",
            FileName = _ffmpegBinary
        };

        _logger.LogDebug($"ffmpeg args:\n${processInfo.Arguments}");

        using Process process = new Process()
        {
            StartInfo = processInfo,
            EnableRaisingEvents = true
        };

        process.ErrorDataReceived += (e, d) => _logger.LogError(d.Data);
        //process.OutputDataReceived += (e, d) =>  _logger.LogInformation(d.Data);

        process.Start();
        process.BeginErrorReadLine();
        //process.BeginOutputReadLine();

        using var inputStream = new MemoryStream(gif.File);
        //using var inputStream = await GetFileStreamAsync("https://media3.giphy.com/media/1lBI2ro8ZmSpWJPAPK/giphy-preview.mp4?cid=587ded4dbfyx3dpwdlosu7r7qaid5e6l0tc7pwrd15e9loi0&ep=v1_gifs_random&rid=giphy-preview.mp4&ct=g");

        await inputStream.CopyToAsync(process.StandardInput.BaseStream);

        process.StandardInput.Close();

        await process.StandardOutput.BaseStream.CopyToAsync(outputStream);
        await process.WaitForExitAsync();

        outputStream.Position = 0;

        var file =  new InputOnlineFile(outputStream, $"{Guid.NewGuid()}.gif");

        if (file.Content == null || file.Content.Length <= 0)
            throw new Exception("Composed file is null or empty");

        return file;
    }



    private Task<Stream> GetFileStreamAsync(string url)
    {
        //download file to stream
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        return http.GetStreamAsync(url);
    }
}