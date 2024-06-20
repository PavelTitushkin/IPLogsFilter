using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IPLogsFilterMVC.Middlewares
{
    public class LogsAuthenticationMiddleware : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public LogsAuthenticationMiddleware(IOptionsMonitor<AuthenticationSchemeOptions> options,
       ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headers = Request.Headers;
            if (!headers.ContainsKey("X-Insider-Id"))
            {
                return AuthenticateResult.Fail("Missing X-Insider-Id header");
            }

            if (!headers.ContainsKey("X-Insider-Permissions"))
            {
                return AuthenticateResult.Fail("Missing X-Insider-Permissions header");
            }

            var userId = headers["X-Insider-Id"].ToString();
            var permissions = headers["X-Insider-Permissions"].ToString().Split(',');

            // Проверка IP-адреса
            var remoteIp = Context.Connection.RemoteIpAddress;
            var validIpRange = "192.168.1.0/24"; // Укажите допустимый диапазон IP
            //if (!IsIpInRange(remoteIp, validIpRange))
            //{
            //    return AuthenticateResult.Fail("IP address is not in the valid range");
            //}

            // Создание claim для аутентификации
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId)
                }.Concat(permissions.Select(permission => new Claim("permission", permission)));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
