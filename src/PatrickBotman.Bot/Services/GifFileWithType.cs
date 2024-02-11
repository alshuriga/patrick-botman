using PatrickBotman.Bot.Models;

namespace PatrickBotman.Services
{
    internal class GifFileWithType
    {
        public GifFileWithType()
        {
        }

        public int Id { get; set; }
        public byte[] File { get; set; }
        public GifType Type { get; set; }
        public string Link { get; set; }
    }
}