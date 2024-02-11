namespace PatrickBotman.Bot.Models;

public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string AdminID { get; init; } = default!;   
    public int LocalGifsProbability { get; init; } = default!;
    public int PollLifetime { get; init; } = default!;
}