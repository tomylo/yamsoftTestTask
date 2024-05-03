using System;
using API.Services.Contracts;
using API.Services.Implementation;
using DAL;
using DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shared.Configurations;



namespace API.Installers
{
    public class IdentityInstaller : IInstaller
    {
        public short Order => 3;
        public void InstallServices(IServiceCollection services, YSConfiguration configuration)
        {
            services.AddIdentity<User, Role>()
                 .AddEntityFrameworkStores<YSDbContext>()
                 .AddDefaultTokenProviders();

            //user related
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });
            services.AddScoped<IIdentityService, IdentityService>();
        }

    }
}
