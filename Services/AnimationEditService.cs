using System.Runtime.InteropServices;
using FFmpeg.NET;
using FFmpeg.NET.Events;
using PatrickBotman.Models;



public class AnimationEditService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _inputPath;
    private readonly string _outputPath;
    private readonly string _ffmpegBinary;
    private readonly int _maximumTextLength;
    private readonly ILogger<AnimationEditService> _logger;


    public AnimationEditService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AnimationEditService> logger)
    {
        var guid = Guid.NewGuid();
        _maximumTextLength = configuration.GetValue<int>("MaximumTextLength");
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

        if (input.Exists)
        {
            TextInput textInput = PrepareText(text);
            _logger.LogInformation("FFmpeg executable registering...");
            var engine = new Engine(_ffmpegBinary);
            // engine.Progress += OnProgress;
            // engine.Data += OnData;
            engine.Error += OnError;
            // engine.Complete += OnComplete;
            var inputFile = new InputFile(input.FullName);
            var outputFile = new OutputFile(_outputPath);
            var maxLineLength = Math.Max(textInput.FirstLine.Length, textInput.SecondLine.Length);
            int fontSize = Math.Min(35, (int)Math.Round((30.0 / (maxLineLength / 20.0))));
            string drawtext = $"drawtext=fontsize={fontSize}:line_spacing=4:font='Impact':text='{textInput.FirstLine}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.1-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
            string? drawtext2 = $"drawtext=fontsize={fontSize}:line_spacing=4:fontfile='Impact':text='{textInput.SecondLine}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.9-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
            Console.WriteLine($"Fontsize: {fontSize}");
            var opts = new ConversionOptions
            {
                ExtraArguments = $"-vf \"scale=300:-1,{String.Join(',', new string[] { drawtext, drawtext2 })}\"",
                VideoFormat = FFmpeg.NET.Enums.VideoFormat.mp4,
                RemoveAudio = true,
                VideoCodec = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? FFmpeg.NET.Enums.VideoCodec.Default : FFmpeg.NET.Enums.VideoCodec.h264_nvenc,
            };
            var cancellationTokenSource = new CancellationTokenSource();
            _logger.LogInformation("Conversion starting...");
            var output = await engine.ConvertAsync(inputFile, outputFile, options: opts, cancellationTokenSource.Token);
            _logger.LogInformation($"Is OUTPUT file exists: {output.FileInfo.Exists}");
            if (output.FileInfo.Exists)
            {
                return output.FileInfo.FullName;
            }
        }
        return null;
    }

    private TextInput PrepareText(string text)
    {
        text = text.Substring(0, Math.Min(_maximumTextLength, text.Length));
        int separationIndex = 0;
        for(int i = 0; i > text.Length; i++) {
            if(text[i] == ' '  || (text[i] == '\n')) {
                if(Math.Abs(text.Length/2 - i) <  Math.Abs(text.Length/2 - separationIndex)) 
                    separationIndex = i;
            }
        }
        string firstLine = text.Substring(0, separationIndex);
        string secondLine = text.Substring(separationIndex, text.Length - 1);
        return new TextInput(firstLine, secondLine);
    }


    public async Task Clean()
    {
        File.Delete(_inputPath);
        File.Delete(_outputPath);
        await Task.CompletedTask;
    }

    private void OnProgress(object sender, ConversionProgressEventArgs e)
    {
        Console.WriteLine("[{0} => {1}]", e.Input.Name, e.Output.Name);
        Console.WriteLine("Bitrate: {0}", e.Bitrate);
        Console.WriteLine("Fps: {0}", e.Fps);
        Console.WriteLine("Frame: {0}", e.Frame);
        Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
        Console.WriteLine("Size: {0} kb", e.SizeKb);
        Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
    }
    private void OnData(object sender, ConversionDataEventArgs e)
    {
        Console.WriteLine("[{0} => {1}]: {2}", e.Input.Name, e.Output.Name, e.Data);
    }

    private void OnComplete(object sender, ConversionCompleteEventArgs e)
    {
        Console.WriteLine("Completed conversion from {0} to {1}", e.Input.Name, e.Output.Name);
    }

    private void OnError(object sender, ConversionErrorEventArgs e)
    {
        Console.WriteLine("[{0} => {1}]: Error: {2}\n{3}", e.Input.Name, e.Output.Name, e.Exception.ExitCode, e.Exception.InnerException);
    }
}