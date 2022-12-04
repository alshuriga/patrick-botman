
using FFmpeg.NET;

public class AnimationEditService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly string _inputPath;
    private readonly string _outputPath;

    public AnimationEditService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;

        var workDir = configuration.GetValue<string>("TempDirectory") ?? string.Empty; ;
        System.IO.Directory.CreateDirectory(workDir);
        
        _inputPath = System.IO.Path.Combine(workDir, "input.mp4");
        _outputPath = System.IO.Path.Combine(workDir, "output.mp4");

    }
    public async Task<string?> AddText(string url, string text)
    {
       if (text.Length > 16) throw new FormatException("Max message length is 16");

        var http = _httpClientFactory.CreateClient();
        using (var filestream = System.IO.File.OpenWrite(_inputPath))
        {
            using (var httpstream = await http.GetStreamAsync(url))
            {
                if (httpstream.CanRead || filestream.CanWrite) ;
                await httpstream.CopyToAsync(filestream);
            }
        }
        var input = new FileInfo(_inputPath);
        if (input.Exists)
        {
            var engine = new Engine("C:\\Program Files (x86)\\Ffmpeg\\ffmpeg-master-latest-win64-gpl\\bin\\ffmpeg.exe");
            var inputFile = new InputFile(input.FullName);
            var outputFile = new OutputFile(_outputPath);
            var opts = new ConversionOptions
            {
                ExtraArguments = $"-vf \"scale=300:-1,drawtext=fontsize=30:fontfile=impact.ttf:text='{text.ToUpper()}':x=(w-text_w)/2:y=(h*0.1-text_h/2):fontcolor=white:bordercolor=black:borderw=3\"",
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