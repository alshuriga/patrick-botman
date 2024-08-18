
namespace PatrickBotman.Common.Services
{
    public interface IUrlMetaService
    {
        Task<string?> GetMetaForUrl(Uri url);
    }
}