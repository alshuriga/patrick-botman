using PatrickBotman.Persistence.Entities;

namespace patrick_botman.Persistence.Entities
{
    public class Blacklist
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public Gif Gif { get; set; } = null!;
    }
}
