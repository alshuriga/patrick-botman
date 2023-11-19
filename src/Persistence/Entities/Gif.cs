using patrick_botman.Persistence.Entities;

namespace PatrickBotman.Persistence.Entities;

public class Gif 
{
    public int Id { get; set; }
    public string GifUrl { get; set; } = null!;
    public List<Blacklist> Blacklist { get; set; } = null!;
}