using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PatrickBotman.AdminPortal.Auth
{
    public class GoogleJwtBearerHandler : JwtBearerHandler
    {
        private ILogger<GoogleJwtBearerHandler> _logger;

        public GoogleJwtBearerHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<GoogleJwtBearerHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var idtoken = Context.Request.Headers.Authorization.First().Split(' ')[1];
                var payload = await GoogleJsonWebSignature.ValidateAsync(idtoken);

                if (Options.TokenValidationParameters.ValidateAudience && (string)payload.Audience != Options.TokenValidationParameters.ValidAudience)
                {
                    throw new Exception("Audience is invalid");
                }
                else if (Options.TokenValidationParameters.ValidateIssuer && !Options.TokenValidationParameters.ValidIssuers.Contains(payload.Issuer))
                {
                    throw new Exception("Issuer is invalid");
                }

                var user = new ClaimsIdentity(ParseClaims(idtoken), "Token");

                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(user), Scheme.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError("Authentication with google id token failed");
                return AuthenticateResult.Fail(ex);
            }
        }

        private IEnumerable<Claim> ParseClaims(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(jwt);

            return token.Claims;
        }
    }
}
