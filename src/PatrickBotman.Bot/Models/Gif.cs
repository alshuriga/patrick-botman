namespace PatrickBotman.Bot.Models
{
    public class Gif
    {
        public int Id { get; set; }
        public byte[] File { get; set; } = null!;
        public GifType Type { get; set; } 
    }

    public enum GifType
    {
        Local, Online
    }
}
