using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using PatrickBotman.Bot.Models;

namespace PatrickBotman.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;
    private readonly ILogger<ConfigureWebhook> _logger;

    public ConfigureWebhook(IServiceProvider services, IConfiguration configuration, ILogger<ConfigureWebhook> logger)
    {
        _services = services;
        _logger = logger;
        _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAdress = @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";

        _logger.LogInformation($"Address: {webhookAdress}");

        await botClient.SetWebhookAsync(
            url: webhookAdress,
            allowedUpdates: new UpdateType[] { UpdateType.Message, UpdateType.InlineQuery, UpdateType.ChosenInlineResult, UpdateType.CallbackQuery },
            cancellationToken: cancellationToken,
            dropPendingUpdates: true
        );

        _logger.LogInformation("Webhook Added");
    }

    async Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

        _logger.LogInformation("Webhook Removed");
    }
}

