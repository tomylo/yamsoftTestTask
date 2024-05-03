using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Shared.Configurations;

namespace API.Installers
{
    public class LoggerInstaller:IInstaller
    {
        public short Order => 8;
        public void InstallServices(IServiceCollection services, YSConfiguration config)
        {
            var logSettings = config.LogSettings;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Debug)
                .WriteTo.File(Path.Combine(logSettings.Folder, "ApiLog.txt"), rollingInterval: logSettings.RollingInterval, fileSizeLimitBytes: 100000)
                .WriteTo.MySQL(config.DbSettings.ConnectionString, logSettings.TableName, logSettings.LogEventLevel, true)
                .CreateLogger();

         
        }
    }
}
