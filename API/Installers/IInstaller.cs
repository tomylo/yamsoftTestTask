using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;

namespace API.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, YSConfiguration configuration);
        //To have option to install services in correct orders.
        short Order { get; }
    }
}
