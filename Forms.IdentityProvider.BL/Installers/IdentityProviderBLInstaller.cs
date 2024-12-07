
using Forms.Common.BL.Facades;
using Forms.Common.Installers;
using Forms.IdentityProvider.BL.MapperProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.IdentityProvider.BL.Installers;

public class IdentityProviderBLInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(AppUserMapperProfile));

        serviceCollection.Scan(selector =>
            selector.FromAssemblyOf<IdentityProviderBLInstaller>()
                .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
    }
}
