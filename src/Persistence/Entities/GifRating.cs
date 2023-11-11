namespace PatrickBotman.Persistence.Entities;

public class GifRating
{
    public int GifRatingId { get; set; }
    public int GifId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public bool Vote { get; set; }
    
}