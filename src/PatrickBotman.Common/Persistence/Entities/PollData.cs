
namespace PatrickBotman.Common.Persistence.Entities
{
    public class PollData
    {
        public int Id { get; set; }
        public string PollChatId { get; set; } = null!;
        public int MessageId { get; set; }
        public GifFile GifFile { get; set; } = null!;
        public int GifFileId { get; set; }
        public List<PollVote> PollVote { get; set; } = null!;
        public DateTime Created { get; set; }
        public bool Closed { get; set; }
    }
}
