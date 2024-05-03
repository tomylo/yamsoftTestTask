using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Shared.Configurations;

namespace API.Installers
{
    public class JwtInstaller : IInstaller
    {
        public short Order => 4;
        public void InstallServices(IServiceCollection services, YSConfiguration config)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.JwtSettings.Secret)),
                ValidateIssuer = true,
                ValidIssuer = config.JwtSettings.Issuer,
                ValidateAudience = true,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ValidAudience = config.JwtSettings.Audience,
                AuthenticationType = config.JwtSettings.Audience,
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}
