using DAL.Models;
using DAL.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace DAL
{
    public class YSDbContext : IdentityDbContext<User, Role, long>
    {
        public Microsoft.EntityFrameworkCore.DbSet<RefreshToken> RefreshTokens { get; set; }

        public YSDbContext() : base()
        {

        }
        public YSDbContext(DbContextOptions builder) : base(builder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = InitConfigForMigration().GetSection("YSConfiguration").GetSection("DbSettings").GetSection("ConnectionString").Value;
                optionsBuilder.UseMySQL(connectionString, builder => builder.CommandTimeout(10000));
            }
            base.OnConfiguring(optionsBuilder);
        }

        private IConfiguration InitConfigForMigration()
        {
            var env= Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configFileName = "ysconfiguration_dev.json"; 
            if (env == Environments.Production)
            {
                configFileName = "ysconfiguration.json";
            }
            Console.WriteLine("=================Using config file ======================");
            Console.WriteLine(@"Configurations\" + configFileName);
            Console.WriteLine("=======================================");
            return new ConfigurationBuilder()
                .AddJsonFile(@"Configurations\"+configFileName,optional:true)
                .Build();
        }


    }
}
