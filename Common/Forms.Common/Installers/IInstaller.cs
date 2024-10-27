using Microsoft.Extensions.DependencyInjection;

namespace Forms.Common.Installers;

public interface IInstaller
{
    void Install(IServiceCollection serviceCollection);
}