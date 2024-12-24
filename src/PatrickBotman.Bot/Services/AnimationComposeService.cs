using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using FFmpeg.NET.Enums;
using PatrickBotman.Bot.Models;
using PatrickBotman.Common.Helpers;
using Telegram.Bot.Types;
using File = System.IO.File;

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

    public async Task<GeneratedGifFile> ComposeGifAsync(GifFileWithType gif, string text)
    {
        var isNewYear = DateTime.Now >= new DateTime(day: 23, month: 12, year: DateTime.Now.Year) || DateTime.Now < new DateTime(day: 7, month: 1, year: DateTime.Now.Year);

        var workDir = _configuration.GetValue<string>("AssetsDirectory")!;

        var guid = Guid.NewGuid();

        //set ffmpeg arguments
        var textInput = new TextInput(text, _configuration);
        var maxLineLength = Math.Max(textInput.FirstLine.Length, textInput.SecondLine.Length);
        string argsTemplate = "drawtext=fontsize=min(((w*0.98)/20)*2\\,((w*0.98)/{0})*2):line_spacing=4:fontfile='assets/impact.ttf':text='{1}':fix_bounds=true:x=(w-text_w)/2:y=(h*{2}-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
        string firstLineArgs = string.Format(argsTemplate, maxLineLength, textInput.FirstLine, 0.1);
        string secondLineArgs = string.Format(argsTemplate, maxLineLength, textInput.SecondLine, 0.9);

        //prepare temporary input file
        var tempInputFilename = $"assets/{guid}_input.mp4";
        var tempOutputFilename = $"assets/{guid}_output.mp4";

        using var tempInputFile = new FileStream(tempInputFilename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.Asynchronous);
        await tempInputFile.WriteAsync(gif.File);


        ProcessStartInfo processInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = isNewYear ? $"-i {tempInputFilename} -i assets/snow.mov -f mp4 -c:v libx264 -filter_complex \"scale=350:-1,pad=ceil(iw/2)*2:ceil(ih/2)*2,overlay=shortest=1,{string.Join(',', new string[] { firstLineArgs, secondLineArgs })}\" {tempOutputFilename}"
                : $"-i {tempInputFilename} -f mp4 -c:v libx264 -an -movflags frag_keyframe+empty_moov -vf \"scale=350:-2,{string.Join(',', new string[] { firstLineArgs, secondLineArgs })}\" {tempOutputFilename}",
            FileName = _ffmpegBinary
        };

        _logger.LogDebug($"ffmpeg args:\n${processInfo.Arguments}");

        using Process process = new Process()
        {
            StartInfo = processInfo,
            EnableRaisingEvents = true,
        };
            process.ErrorDataReceived += (e, d) =>
            {
               if (d.Data != null && d.Data.ToLower().Contains("error"))
                {
                    _logger.LogCritical(d.Data);
                    //throw new FormatException($"ffmpeg Error: {d.Data}");
                }
            };

            process.Start();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            byte[] buffer = null!;

            using (var outputStream = new MemoryStream())
            {
                using (var fileStream = new FileStream(tempOutputFilename, FileMode.Open))
                {
                    await fileStream.CopyToAsync(outputStream);

                    buffer = outputStream.ToArray();           
                }
            }


            var file = new GeneratedGifFile()
            {
                Name = $"{Guid.NewGuid()}_{gif.Type}_{gif.Id}.mp4",
                Data = buffer
            };


            File.Delete(tempInputFilename);
            File.Delete(tempOutputFilename);

            return file;

      
    }
}