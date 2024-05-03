using Contracts.V1.Requests.Auth;
using Contracts.V1.Response.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await GetData();

            var m=GetInput("'more' if you can try more");
            if (m == "more")
            {
                await GetData();
            }
            Console.WriteLine("To close app press any key!");
            Console.ReadKey();
        }

        //get data needed for login
        static async Task GetData()
        {
            var login = GetInput("login");
            var password = GetInput("password");

            var apiUrl = InitConfig().GetSection("YSConfiguration").GetSection("GenericSettings").GetSection("ApiServer").Value;
            if (string.IsNullOrEmpty(apiUrl))
            {
                PrintConsole("API Url is missing in configuration file", ConsoleColor.Red);
                apiUrl = GetInput("API url");
            }

            PrintConsole("Trying to loggin", ConsoleColor.Yellow);

            await Login(apiUrl, login, password);
        }

        //execute login 
        static async Task Login(string apiUrl,string login,string password)
        {
            var client = new ApiClient(apiUrl);

            var authRequest= new AuthRequest() { Email = login, Password = password };
           
            try
            {
                var result = await client.PostDataAsync("/api/v1/login/", authRequest);
                if (string.IsNullOrEmpty(result))
                {
                    PrintConsole($"Error during login.Try later", ConsoleColor.Red);
                    return;
                }
                var authRes=JsonConvert.DeserializeObject<AuthResponse>(result);
                if (!authRes.IsSuccess)
                {
                    PrintConsole(string.Join(Environment.NewLine,authRes.Errors), ConsoleColor.Red);
                    return;
                }
                else
                {
                    PrintConsole($"Succesfully logIn! Your id is {authRes.MyId}", ConsoleColor.Green);

                }

            }
            catch (Exception error)
            {
                PrintConsole($"Error during login. {error.Message}", ConsoleColor.Red);
            }
        }

        //Method write to user and get info from user
        static string GetInput(string inputtype)
        {
            Console.WriteLine($"Please write {inputtype} and press Enter");
            var input = Console.ReadLine();
            while(string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"Please write {inputtype} and press Enter");
                input = Console.ReadLine();
            }
            return input;
        }

        //a bit shortest way to write differnt color
        private static void PrintConsole(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        //init configuration to get API url
        private static  IConfiguration InitConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configFileName = "ysconfiguration_dev.json";
            if (env == Environments.Production)
            {
                configFileName = "ysconfiguration.json";
            }
            Console.WriteLine("=================Using config file ======================");
            Console.WriteLine(@"Configurations\" + configFileName);
            Console.WriteLine("=======================================");
            return new ConfigurationBuilder()
                .AddJsonFile(@"Configurations\" + configFileName, optional: true)
                .Build();
        }

        

    }
    
}
