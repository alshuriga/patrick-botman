namespace PatrickBotman.Bot.Interfaces;

public interface IGifProvider
{
    public Task<string> RandomTrendingAsync();
}