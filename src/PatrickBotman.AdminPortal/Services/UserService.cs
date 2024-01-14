
using Microsoft.EntityFrameworkCore;
using PatrickBotman.Common.Persistence;

namespace PatrickBotman.AdminPortal.Services
{
    public class UserService : IUserService
    {
        private readonly PatrickBotmanContext _context; 

        public UserService(PatrickBotmanContext context)
        {
            _context = context;
        }

        public async Task<bool> IsAdmin(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.IsAdmin);
        }
    }
}
