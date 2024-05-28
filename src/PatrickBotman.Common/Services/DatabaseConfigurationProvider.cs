using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using PatrickBotman.Common.Persistence;

namespace PatrickBotman.Common.Services
{
    public class DatabaseConfigurationProvider : ConfigurationProvider
    {

        private DateTime LastLoaded = DateTime.UtcNow;

        private readonly TimeSpan _refreshFrequency;

        private readonly DbContextOptionsBuilder<PatrickBotmanContext> _ctxBuilder;


        public DatabaseConfigurationProvider(Action<DbContextOptionsBuilder> options, TimeSpan refreshFrequency)
        {
            _refreshFrequency = refreshFrequency;

            _ctxBuilder = new DbContextOptionsBuilder<PatrickBotmanContext>();
            options(_ctxBuilder);
        }

        public override void Load()
        {    
            using (var dbContext = new PatrickBotmanContext(_ctxBuilder.Options))
            {
                Data = dbContext.ConfigEntries.ToDictionary(k => k.Name, v => v.Value);
            }
        }

        public override bool TryGet(string key, out string value)
        {
            if(LastLoaded.Add(_refreshFrequency) < DateTime.UtcNow)
            {
                Load();
            }

            return base.TryGet(key, out value);
        }
    }
}
