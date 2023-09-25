using Microsoft.EntityFrameworkCore;
using PatrickBotman.Interfaces;
using PatrickBotman.Models;
using PatrickBotman.Persistence;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Services;

public class GifRatingRepository : IGifRatingRepository
{
    private readonly GifRatingsContext _context;

    public GifRatingRepository(GifRatingsContext context)
    {
        _context = context;
    }
    public async Task DownvoteGifAsync(int gifId, long userId, long chatId)
    {
        var rating = await _context.GifRatings.Where(r => r.GifId == gifId && r.UserId == userId && r.ChatId == chatId).SingleOrDefaultAsync();

        switch(rating?.Vote) 
        {
            case null:
              await _context.GifRatings.AddAsync(new GifRating() {
                GifId = gifId,
                UserId = userId,
                ChatId = chatId,
                Vote = false});

                break;

            case false:
                _context.GifRatings.Remove(rating);
                break;

            case true:
                rating.Vote = false;
                break;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetGifIdAsync(string gifUrl)
    {
        var gif = await _context.Gifs.SingleOrDefaultAsync(x => x.GifUrl == gifUrl);
        if(gif != null) return gif.GifId;
        
        var newGif = new Gif() { GifUrl = gifUrl};
        await _context.Gifs.AddAsync(newGif);

        await _context.SaveChangesAsync();

        return newGif.GifId;
    }

    public async Task<int> GetGifRatingAsync(int gifId, long chatId)
    {
        var rating = await _context.Gifs
        .Include(x => x.GifRatings).AsNoTracking()
        .SelectMany(x => x.GifRatings, (gif, rating) => new {id = gif.GifId, rating = rating.Vote, chatId })
        .Where(x => x.chatId == chatId)
        .GroupBy(x => x.id,
            x => x.rating,
            (id, vote) => new { gifId = id, rating = vote.Count(v => v) - vote.Count(v => !v)})
            .FirstOrDefaultAsync(x => x.gifId == gifId);

        return rating?.rating ?? 0;
    }

    public async Task<GifDTO?> GetRandomGifAsync(long chatId)
    {
        var gifIds = await _context.Gifs
        .Include(x => x.GifRatings)
        .SelectMany(x => x.GifRatings, (gif, rating) => new { id = gif.GifId, url = gif.GifUrl, rating = rating.Vote, chatId })
        .Where(x => x.chatId == chatId)
        .GroupBy(x => new { x.url, x.id },
            x => x.rating,
            (gifData, vote) => new { gifId = gifData.id, gifUrl = gifData.url, goodRating = vote.Count(v => v) > vote.Count(v => !v)})
                .Where(x => x.goodRating)
                .Select(x => new { id = x.gifId, url = x.gifUrl})
                .ToListAsync();
        
        var rnd = new Random();
        if(gifIds.Count <= 0) return null;

        var gif =  gifIds.ElementAtOrDefault(new Random().Next(0, gifIds.Count - 1));

        if (gif == null) return null;

        return new GifDTO(gif.id, gif.url);
    }

    public async Task<string> GetUrlById(int gifId)
    {
        var gif = await _context.Gifs.SingleAsync(x => x.GifId == gifId);
        return gif.GifUrl;
    }

    public async Task UpvoteGifAsync(int gifId, long userId, long chatId)
    {
        var rating = await _context.GifRatings.Where(r => r.GifId == gifId && r.UserId == userId && r.ChatId == chatId).SingleOrDefaultAsync();

        switch(rating?.Vote) 
        {
            case null:
              await _context.GifRatings.AddAsync(new GifRating() {
                GifId = gifId,
                UserId = userId,
                ChatId = chatId,
                Vote = true});

                break;

            case true:
                _context.GifRatings.Remove(rating);
                break;

            case false:
                rating.Vote = true;
                break;
        }

        await _context.SaveChangesAsync();
    }
}
