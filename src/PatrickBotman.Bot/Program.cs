using PatrickBotman.Services;
using PatrickBotman.Bot.Models;
using Telegram.Bot;
using PatrickBotman.Bot.UpdateHandlers;
using PatrickBotman.Bot.Interfaces;
using PatrickBotman.Common.Helpers;
using PatrickBotman.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();



builder.Services.AddHttpClient("tgwebhook").
    AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        TelegramBotClientOptions opts = new(botConfig.BotToken);
        return new TelegramBotClient(opts, httpClient);
    } );

// builder.Services.AddHttpClient("tenorclient", tenorclient => {
//     var tenorConfiguration = builder.Configuration.GetSection("TenorConfiguration").Get<TenorConfiguration>();
//     tenorclient.BaseAddress = new Uri($"{tenorConfiguration.HostAddress}?key={tenorConfiguration.ApiToken}");
// });

builder.Services.AddHttpClient("giphyclient", giphyclient => {
    var giphyConfiguration = builder.Configuration.GetSection("giphyConfiguration").Get<GiphyConfiguration>();
    giphyclient.BaseAddress = new Uri($"{giphyConfiguration.HostAddress}?api_key={giphyConfiguration.ApiToken}");
});

builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddScoped<IGifProvider, GiphyGIfProvider>();
builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScoped<AnimationEditService>();
builder.Services.AddScoped<FileDownloaderService>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<UpdateHandlersFactory>();


var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints => {
    var token = botConfig.BotToken;
    endpoints.MapControllerRoute(name: "tgwebhook",
    pattern: $"bot/{token}",
    new { controller = "WebHook", action = "Post" }
    );
    endpoints.MapControllers();
});


app.Run();
