using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FFmpeg.NET;
using FFmpeg.NET.Events;
using PatrickBotman.Models;

namespace PatrickBotman.Services;

public class AnimationEditService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _inputPath;
    private readonly string _outputPath;
    private readonly string _ffmpegBinary;
    private readonly IConfiguration _configuration;

    private readonly ILogger<AnimationEditService> _logger;


    public AnimationEditService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AnimationEditService> logger)
    {
        _configuration = configuration;
        var guid = Guid.NewGuid();
        _httpClientFactory = httpClientFactory;
        _ffmpegBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
           ? "ffmpeg"
           : configuration.GetValue<string>("FfmpegBinary");
        var workDir = configuration.GetValue<string>("TempDirectory") ?? string.Empty; ;

        System.IO.Directory.CreateDirectory(workDir);
        _inputPath = System.IO.Path.Combine(workDir, $"{guid}_input.mp4");
        _outputPath = System.IO.Path.Combine(workDir, $"{guid}_output.mp4");
        _logger = logger;

    }

    public async Task<string?> AddText(string url, string text)
    {
        _logger.LogInformation("Downloading GIF from Tenor...");
        
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(10);
        using (var filestream = System.IO.File.OpenWrite(_inputPath))
        {
            using (var httpstream = await http.GetStreamAsync(url))
            {
                if (httpstream.CanRead || filestream.CanWrite)
                    await httpstream.CopyToAsync(filestream);
            }
        }

        var input = new FileInfo(_inputPath);

        _logger.LogInformation($"Is INPUT file exists: {input.Exists}");

        if (!input.Exists) return null;
        var textInput = new TextInput(text, _configuration);

        _logger.LogInformation("FFmpeg executable registering...");

        var engine = new Engine(_ffmpegBinary);
        engine.Error += OnError!;
        engine.Complete += OnComplete!;
        engine.Data += OnData!;

        var inputFile = new InputFile(input.FullName);
        var outputFile = new OutputFile(_outputPath);

        var maxLineLength = Math.Max(textInput.FirstLine.Length, textInput.SecondLine.Length);
        int fontSize = Math.Min(35, (295 / maxLineLength) * 2);
        _logger.LogInformation($"Font Size: {fontSize}");

        
        // string firstLineArgs = $"drawtext=fontsize={fontSize}:line_spacing=4:font='Impact':text='{textInput.FirstLine}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.1-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
        // string secondLineArgs = $"drawtext=fontsize={fontSize}:line_spacing=4:fontfile='Impact':text='{textInput.SecondLine}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.9-text_h/2):fontcolor=white:bordercolor=black:borderw=3";

        string firstLineArgs = $"drawtext=fontsize={fontSize}:line_spacing=4:font='Impact':text='test':fix_bounds=true:x=(w-text_w)/2:y=(h*0.1-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
        string secondLineArgs = $"drawtext=fontsize={fontSize}:line_spacing=4:fontfile='Impact':text='test':fix_bounds=true:x=(w-text_w)/2:y=(h*0.9-text_h/2):fontcolor=white:bordercolor=black:borderw=3";

        _logger.LogInformation($"firstLineArgs: {firstLineArgs}\nsecondLineArgs: {secondLineArgs}");



        var opts = new ConversionOptions
        {
            ExtraArguments = $"-vf \"scale=300:-1,{String.Join(',', new string[] { firstLineArgs, secondLineArgs })}\"",
            VideoFormat = FFmpeg.NET.Enums.VideoFormat.mp4,
            RemoveAudio = true,
            VideoCodec = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? FFmpeg.NET.Enums.VideoCodec.Default : FFmpeg.NET.Enums.VideoCodec.h264_nvenc,
        };

        var cancellationTokenSource = new CancellationTokenSource();

        _logger.LogInformation("Conversion starting...");

        try
        {
            var output = await engine.ConvertAsync(inputFile, outputFile, options: opts, cancellationTokenSource.Token);
            _logger.LogInformation($"Is OUTPUT file exists: {output.FileInfo.Exists}");
            if (!output.FileInfo.Exists) return null;
            return output.FileInfo.FullName;
        }
        catch (Exception ex)
        {
            _logger.LogError($"CONVERSION ERROR:\n{ex.Source}\n{ex.Message}\n{ex.StackTrace}\nMessage: {ex.InnerException?.Message}\n{ex.InnerException?.InnerException?.Message}");
            return null;
        }
    }



    public async Task Clean()
    {
        File.Delete(_inputPath);
        File.Delete(_outputPath);
        await Task.CompletedTask;
    }

    private void OnProgress(object sender, ConversionProgressEventArgs e)
    {
         _logger.LogDebug($"Data {e.Input.Argument}");
    }
    private void OnData(object sender, ConversionDataEventArgs e)
    {
       _logger.LogDebug($"Data {e.Data}");
    }

    private void OnComplete(object sender, ConversionCompleteEventArgs e)
    {
        _logger.LogDebug($"{e.Output.Argument}, {e.Input.Argument}");
    }

    private void OnError(object sender, ConversionErrorEventArgs e)
    {
        _logger.LogCritical("[{0} => {1}]: Error: {2}\n{3}\n{4}", e.Input.Name, e.Output.Name,
        e.Exception.ExitCode, e.Exception.Message, e.Exception.InnerException?.Message);
        throw new ApplicationException("Error while converting file");
    }
}