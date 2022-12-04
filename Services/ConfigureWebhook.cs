using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using PatrickBotman.Models;

namespace PatrickBotman.Services;

public class ConfigureWebhook : IHostedService
{   
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;
    
    public ConfigureWebhook(IServiceProvider services, IConfiguration configuration)
    {
        _services = services;
        _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAdress = @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";
        await botClient.SetWebhookAsync(
            url: webhookAdress,
            allowedUpdates: new UpdateType[] { UpdateType.Message, UpdateType.InlineQuery, UpdateType.ChosenInlineResult },
            cancellationToken: cancellationToken
        );

    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}

