using Forms.Api.BL.Facades;
using Forms.Common.BL.Facades;
using Forms.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.Api.BL.Installers;

public class ApiBLInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(selector =>
            selector.FromAssemblyOf<ApiBLInstaller>()
                .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        serviceCollection.AddScoped<SearchFacade>(); 
    }
}