using DAL;
using DAL.Models;
using DBLogic.Contracts;
using DBLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shared.Configurations;

using static Org.BouncyCastle.Math.EC.ECCurve;



namespace API.Installers
{
    public class DbRepositoryInstaller:IInstaller
    {
        public short Order => 2;
        public void InstallServices(IServiceCollection services, YSConfiguration configuration)
        {
            // add Db Repositories 
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            //add Db repo
			services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
