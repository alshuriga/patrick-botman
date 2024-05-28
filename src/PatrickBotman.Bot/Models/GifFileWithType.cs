namespace PatrickBotman.Bot.Models
{
    public class GifFileWithType
    {
        public int Id { get; set; }
        public byte[] File { get; set; } = null!;
        public GifType Type { get; set; }
        public string? Link { get; set; }
    }

    public enum GifType
    {
        Local, Online
    }
}
