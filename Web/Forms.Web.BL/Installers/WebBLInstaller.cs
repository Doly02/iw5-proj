using System;
using System.Net.Http;
using Forms.Common.BL.Facades;
using Forms.Web.BL.Facades;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.Web.BL.Installers
{
    public class WebBLInstaller
    {
        public void Install(IServiceCollection serviceCollection, string apiBaseUrl)
        {
            serviceCollection.AddScoped<IQuestionApiClient, QuestionApiClient>();

            serviceCollection.AddScoped<IResponseApiClient, ResponseApiClient>();
            
            serviceCollection.AddScoped<Func<string, IUserApiClient>>(serviceProvider => clientName =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(clientName);
                return new UserApiClient(httpClient, apiBaseUrl);
            });

            
            serviceCollection.AddScoped<IFormApiClient, FormApiClient>();
            
            serviceCollection.AddScoped<ISearchApiClient, SearchApiClient>();

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<WebBLInstaller>()
                    .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime());
            
            serviceCollection.AddTransient<AppUserFacade, AppUserFacade>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("IdentityServer");
                return new AppUserFacade(client);
            });
        }
        
    }
}