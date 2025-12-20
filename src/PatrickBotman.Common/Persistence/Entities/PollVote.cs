namespace PatrickBotman.Common.Persistence.Entities
{
    public class PollVote
    {
        public int Id { get; set; }
        public PollData PollData { get; set; } = null!;
        public long UserId { get; set; }
        public int Vote { get; set; } 
    }
}
