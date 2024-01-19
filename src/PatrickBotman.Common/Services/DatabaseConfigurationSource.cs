using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PatrickBotman.Common.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Services
{
    internal class DatabaseConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public DatabaseConfigurationSource(Action<DbContextOptionsBuilder> optionsAction) => _optionsAction = optionsAction;

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new DatabaseConfigurationProvider(_optionsAction, TimeSpan.FromMinutes(5));
    }
}
