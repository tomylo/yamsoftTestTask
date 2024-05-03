using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;


namespace API.Installers
{
    public class DbInstaller : IInstaller
    {
        public short Order => 1;
        public void InstallServices(IServiceCollection services, YSConfiguration config)
        {
            services.AddDbContext<YSDbContext>(options =>
            {
                options.UseMySQL(config.DbSettings.ConnectionString);
            });

        }

      
    }
}
