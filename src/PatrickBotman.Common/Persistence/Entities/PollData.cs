
namespace PatrickBotman.Common.Persistence.Entities
{
    public class PollData
    {
        public int Id { get; set; }
        public string PollId { get; set; } = null!;
        public long PollChatId { get; set; }
        public GifFile GifFile { get; set; } = null!;
        public int GifFileId { get; set; }
    }
}
