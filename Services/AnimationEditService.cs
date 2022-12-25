
using FFmpeg.NET;
using PatrickBotman.Helpers;

public class AnimationEditService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly string _inputPath;
    private readonly string _outputPath;


    public AnimationEditService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        var guid = Guid.NewGuid();

        _httpClientFactory = httpClientFactory;

        var workDir = configuration.GetValue<string>("TempDirectory") ?? string.Empty; ;
        System.IO.Directory.CreateDirectory(workDir);
        
        _inputPath = System.IO.Path.Combine(workDir, $"{guid}_input.mp4");
        _outputPath = System.IO.Path.Combine(workDir, $"{guid}_output.mp4");
        
    }
    public async Task<string?> AddText(string url, string text)
    { 
        var http = _httpClientFactory.CreateClient();
        using (var filestream = System.IO.File.OpenWrite(_inputPath))
        {
            using (var httpstream = await http.GetStreamAsync(url))
            {
                if (httpstream.CanRead || filestream.CanWrite)
                    await httpstream.CopyToAsync(filestream);
            }
        }
        var input = new FileInfo(_inputPath);
        if (input.Exists)
        {
            var engine = new Engine("C:\\Program Files (x86)\\Ffmpeg\\ffmpeg-master-latest-win64-gpl\\bin\\ffmpeg.exe");
            var inputFile = new InputFile(input.FullName);
            var outputFile = new OutputFile(_outputPath);
            var formattedText = text.Split("\n", options: StringSplitOptions.RemoveEmptyEntries);
            var maxLineLength = formattedText.Max(s => s.Length);
            int fontSize = (int)Math.Round((30.0 / (maxLineLength / 15.0)));
            string drawtext = $"drawtext=fontsize={fontSize}:line_spacing=4:fontfile=impact.ttf:text='{(formattedText.ElementAtOrDefault(0) ?? String.Empty).ToUpper()}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.1-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
            string? drawtext2 = $"drawtext=fontsize={fontSize}:line_spacing=4:fontfile=impact.ttf:text='{(formattedText.ElementAtOrDefault(1)  ?? String.Empty).ToUpper()}':fix_bounds=true:x=(w-text_w)/2:y=(h*0.9-text_h/2):fontcolor=white:bordercolor=black:borderw=3";
            
            Console.WriteLine($"Fontsize: {fontSize}");
            var opts = new ConversionOptions
            {
                ExtraArguments = $"-vf \"scale=300:-1,{String.Join(',', new string[]{ drawtext, drawtext2 })}\"",
                VideoFormat = FFmpeg.NET.Enums.VideoFormat.mp4,
                RemoveAudio = true,
                VideoCodec = FFmpeg.NET.Enums.VideoCodec.h264_nvenc,
            };
            var cancellationTokenSource = new CancellationTokenSource();
            var output = await engine.ConvertAsync(inputFile, outputFile, options: opts, cancellationTokenSource.Token);
            if (output.FileInfo.Exists)
            {
                return output.FileInfo.FullName;
            }
        }
        return null;
    }


    public async Task Clean()
    {
        File.Delete(_inputPath);
        File.Delete(_outputPath);     
        await Task.CompletedTask;
    }
}