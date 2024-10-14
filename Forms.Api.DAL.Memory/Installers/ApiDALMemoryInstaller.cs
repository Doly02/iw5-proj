using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.Api.DAL.Memory.Installers;

public class ApiDALMemoryInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(selector =>
            selector.FromAssemblyOf<ApiDALMemoryInstaller>()
                .AddClasses(classes => classes.AssignableTo(typeof(IApiRepository<>)))
                .AsMatchingInterface()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo<Storage>())
                .AsSelf()
                .WithSingletonLifetime()
        );
    }
}