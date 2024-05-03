
using API.Helpers;
using API.Installers;
using API.Middleware;
using DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Shared.Configurations;


namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var initConfig = InitConfig();
            IConfigurationSection sec = initConfig.GetSection(nameof(YSConfiguration));
            var config = new YSConfiguration();
            sec.Bind(config);
            builder.Services.Configure<YSConfiguration>(sec);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMvc();
            //install all services inheritance IInstaller interface
            builder.Services.InstallServicesInAssembly(config);

            builder.Host.UseSerilog();

            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.MapControllers();
            //register midddlware for handle exceptions.
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseStaticFiles();
            
            await SeedData(app.Services, config);
            app.Run();
        }

        /// <summary>
        /// Check and seed data if needed.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static async Task SeedData(IServiceProvider serviceProvider,YSConfiguration config)
        {
            //to avoid constant checking of the Seed Data, this can be disabled via configuration.
            if (config.GenericSettings.CheckForSeedData)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<Role>>();
                    await IdentityDataSeed.SeedData(userManager, roleManager);
                }
            }
        }

        /// <summary>
        /// Loading config file
        /// </summary>
        /// <param name="fileShouldExists"></param>
        /// <returns>IConfigurationRoot or throws an exception if the configuration file is not found, because without configuration the API cannot be started and operated.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static IConfigurationRoot InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configFileName = "ysconfiguration_dev.json";
            if (env == Environments.Production) {
                configFileName = "ysconfiguration.json";
            }
           
            var configFilePath = @"Configurations\" + configFileName;
        
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\"+configFilePath))
            {
                throw new FileNotFoundException(configFilePath);
            }
            
            Console.WriteLine("=================Using configuration file ======================");
            Console.WriteLine(configFilePath);
            Console.WriteLine("=======================================");
            return new ConfigurationBuilder()
                .AddJsonFile(configFilePath, optional: true)
                .Build();
        }
    }
}
