
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatrickBotman.Common.Interfaces;
using PatrickBotman.Common.Persistence;
using PatrickBotman.Services;

namespace PatrickBotman.Common.Helpers
{
    public static class Configuration
    {
        public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatrickBotmanContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("PostgreConn")!));
            services.AddScoped<IOnlineGifRepository, OnlineGifRepository>();
            services.AddScoped<ILocalGifRepository, LocalGifRepository>();
            services.AddScoped<IPollDataRepository, PollDataRepository>();
            services.AddScoped<IChatsRepository, ChatsRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            
            return services;
        }
    }
}
