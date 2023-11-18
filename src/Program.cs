using PatrickBotman.Interfaces;
using PatrickBotman.Services;
using PatrickBotman.Models;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();


builder.Services.AddHttpClient("tgwebhook").
    AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        TelegramBotClientOptions opts = new(botConfig.BotToken);
        return new TelegramBotClient(opts, httpClient);
    } );

builder.Services.AddHttpClient("giphyclient", giphyclient => {
    var giphyConfiguration = builder.Configuration.GetSection("giphyConfiguration").Get<GiphyConfiguration>();
    giphyclient.BaseAddress = new Uri($"{giphyConfiguration.HostAddress}?api_key={giphyConfiguration.ApiToken}");
});


builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddScoped<IGifService, GiphyService>();

builder.Services.AddScoped<HandleUpdateService>();

builder.Services.AddScoped<AnimationEditService>();

builder.Services.AddScoped<FileDownloaderService>();

builder.Services.AddControllers().AddNewtonsoftJson();


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
