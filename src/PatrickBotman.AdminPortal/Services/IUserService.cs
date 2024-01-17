namespace PatrickBotman.AdminPortal.Services
{
    public interface IUserService
    {
        Task<bool> IsAdmin(string userId);
    }
}
