using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PatrickBotman.Bot.Models;

public class HeaderAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "HeaderAuth";
    private readonly BotConfiguration _botConfig;

    public HeaderAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IOptionsSnapshot<BotConfiguration> botConfiguration)
        : base(options, logger, encoder, clock)
    {
        _botConfig = botConfiguration.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var headerValue))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
        }

        if (headerValue != _botConfig.ExternalToken)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "HeaderUser") };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
