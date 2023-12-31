namespace PatrickBotman.Bot.Models;

public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public ICollection<long> AdminID { get; init; } = default!;
}