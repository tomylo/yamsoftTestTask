
using Shared.Configurations;
using static System.Net.Mime.MediaTypeNames;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
         
            var initConfig = InitConfig(true);
            IConfigurationSection sec = initConfig.GetSection(nameof(YSConfiguration));
            var config = new YSConfiguration();
            sec.Bind(config);
            builder.Services.Configure<YSConfiguration>(sec);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMvc();


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

            app.Run();
        }


        //load configrations from file 
        private static IConfigurationRoot InitConfig(bool fileShouldExists=false)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configFileName = "ysconfiguration_dev.json";
            if (env == Environments.Production) {
                configFileName = "ysconfiguration.json";
            }
           
            var configFilePath = @"Configurations\" + configFileName;
            if (fileShouldExists)
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\"+configFilePath))
                {
                    throw new FileNotFoundException(configFilePath);
                }
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
