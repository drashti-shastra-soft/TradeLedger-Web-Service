using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Text;
using Owin;

namespace TradeLedgerWebService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["SecretKey"]);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ConfigurationManager.AppSettings.Get("Issuer"),
                    ValidAudience = ConfigurationManager.AppSettings.Get("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RequireExpirationTime = true,
                }
            });
        }
    }
}