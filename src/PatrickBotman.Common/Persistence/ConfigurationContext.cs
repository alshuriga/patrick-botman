using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence.Entities;

namespace PatrickBotman.Common.Persistence;

public class ConfigurationContext : DbContext
{
    public DbSet<ConfigEntry> ConfigEntries { get; set; } = null!;

    public ConfigurationContext(DbContextOptions<ConfigurationContext> dbContextOptions) : base(dbContextOptions)
    {}
}