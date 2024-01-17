using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using PatrickBotman.AdminPortal.Services;

namespace PatrickBotman.AdminPortal.Auth
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminAuthorizationRequirement>
    {
        private readonly IUserService _userService;

        public AdminAuthorizationHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAuthorizationRequirement requirement)
        {
            var email = context.User.Claims.FirstOrDefault(x => x.Type == "email");

            if(email?.Value != null && await _userService.IsAdmin(email.Value)) {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
            return;
        }
    }
}
