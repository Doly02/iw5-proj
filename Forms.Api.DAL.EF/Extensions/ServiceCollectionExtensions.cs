using Forms.Api.DAL.EF.Installers;
using Forms.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.Api.DAL.EF.Extensions
{
    
    public static class ServiceCollectionExtensions
    {
        public static void AddInstaller<TInstaller>(this IServiceCollection serviceCollection, string connectionString)
            where TInstaller : ApiDALEFInstaller, new()
        {
            var installer = new TInstaller();
            installer.Install(serviceCollection, connectionString);
        }
    }
}