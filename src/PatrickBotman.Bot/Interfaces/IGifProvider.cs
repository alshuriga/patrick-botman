using PatrickBotman.Bot.Models;

namespace PatrickBotman.Bot.Interfaces;

public interface IGifProvider
{
    public Task<Gif> RandomGifAsync(long chatId);
}