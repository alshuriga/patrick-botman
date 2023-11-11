namespace PatrickBotman.Persistence.Entities;

public class Gif 
{
    public int GifId { get; set; }
    public string GifUrl { get; set; } = null!;
    public ICollection<GifRating> GifRatings { get; set; } = new List<GifRating>();
}