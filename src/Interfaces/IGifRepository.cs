using PatrickBotman.Models;
using PatrickBotman.Models.DTOs;
using PatrickBotman.Persistence.Entities;

namespace PatrickBotman.Interfaces;

public interface IGifRepository
{
    Task<bool> IsBlacklistedAsync(string gifUrl, long chatId);
    Task BlacklistAsync(int id, long chatId);
    Task<int> GetIdOrCreateAsync(string gifUrl);
}