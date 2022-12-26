using PatrickBotman.Services;
using PatrickBotman.Models;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));

builder.Services.AddHttpClient("tenorclient", tenorclient => {
    var tenorConfiguration = builder.Configuration.GetSection("TenorConfiguration").Get<TenorConfiguration>();
    tenorclient.BaseAddress = new Uri($"{tenorConfiguration.HostAddress}?key={tenorConfiguration.ApiToken}");
});

builder.Services.AddScoped<TenorService>();

builder.Services.AddScoped<HandleUpdateService>();

builder.Services.AddScoped<AnimationEditService>();

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
