using PatrickBotman.Services;
using PatrickBotman.Bot.Models;
using Telegram.Bot;
using PatrickBotman.Bot.UpdateHandlers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Common.Helpers;
using PatrickBotman.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("giphyclient", giphyclient => {
    var giphyConfiguration = builder.Configuration.GetSection("giphyConfiguration").Get<GiphyConfiguration>();
    giphyclient.BaseAddress = new Uri($"{giphyConfiguration.HostAddress}?api_key={giphyConfiguration.ApiToken}");
});

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection(nameof(BotConfiguration)));
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddScoped<IGifProvider, GIfProvider>();
builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScoped<AnimationComposeService>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<UpdateHandlersFactory>();

builder.Services.AddHttpClient("tgwebhook").
    AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var botToken = builder.Configuration.GetSection("BotConfiguration:BotToken").Value;

        TelegramBotClientOptions opts = new(botToken);
        return new TelegramBotClient(opts, httpClient);
    });

var app = builder.Build();


app.UseRouting();

app.UseEndpoints(endpoints => {
    var token = builder.Configuration.GetSection("BotConfiguration:BotToken").Value;
    endpoints.MapControllerRoute(name: "tgwebhook",
    pattern: $"bot/{token}",
    new { controller = "WebHook", action = "Post" }
    );
    endpoints.MapControllers();
});


app.Run();
