using Shared.Configurations;

namespace API.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, YSConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(i =>
                typeof(IInstaller).IsAssignableFrom(i) && !i.IsAbstract && !i.IsInterface).Select(Activator.CreateInstance).Cast<IInstaller>().OrderBy(a=>a.Order).ToList();
            
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
