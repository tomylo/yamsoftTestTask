using API.Services.Contracts;
using API.Services.Implementation;
using Shared.Configurations;

namespace API.Installers
{
    public class ServicesInstaller:IInstaller
    {
        public short Order =>4;

        
        public void InstallServices(IServiceCollection services, YSConfiguration configuration)
        {
            services.AddScoped<IUserNotifications, UserNotifications>();
      
        }
    }
}
