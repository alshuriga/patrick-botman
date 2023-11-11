using Microsoft.EntityFrameworkCore;
using PatrickBotman.Interfaces;
using PatrickBotman.Models;
using PatrickBotman.Persistence;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Services;

public class GifRatingService : IGifRatingService
{
    private readonly GifRatingsContext _context;

    public GifRatingService(GifRatingsContext context)
    {
        _context = context;
    }


    public async Task<int> GetOrCreateIdForGifUrlAsync(string gifUrl)
    {
        var gif = await _context.Gifs.AsNoTracking().SingleOrDefaultAsync(x => x.GifUrl == gifUrl);

        if(gif != null) return gif.GifId;
        
        var newGif = new Gif() { GifUrl = gifUrl};

        await _context.Gifs.AddAsync(newGif);

        await _context.SaveChangesAsync();

        return newGif.GifId;
    }

    public async Task<int> GetGifRatingByIdAsync(int gifId, long chatId)
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
        .AsNoTracking()
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
        var gif = await _context.Gifs.AsNoTracking().SingleAsync(x => x.GifId == gifId);
        return gif.GifUrl;
    }
    
    public async Task RateGifAsync(bool upvote, int gifId, long userId, long chatId)
    {
        var rating = await _context.GifRatings.Where(r => r.GifId == gifId && r.UserId == userId && r.ChatId == chatId).SingleOrDefaultAsync();

        switch(rating?.Vote) 
        {
            case null:
              await _context.GifRatings.AddAsync(new GifRating() {
                GifId = gifId,
                UserId = userId,
                ChatId = chatId,
                Vote = upvote});

                break;

            case false:
                if(!upvote)
                    _context.GifRatings.Remove(rating);
                else
                    rating.Vote = upvote;
                break;

            case true:
                if(upvote)
                    _context.GifRatings.Remove(rating);
                else
                    rating.Vote = upvote;
                break;
        }

        await _context.SaveChangesAsync();
    }
}
