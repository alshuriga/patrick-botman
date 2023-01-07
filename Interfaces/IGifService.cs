namespace PatrickBotman.Interfaces;

public interface IGifService
{
    public Task<string> RandomTrendingAsync();
}