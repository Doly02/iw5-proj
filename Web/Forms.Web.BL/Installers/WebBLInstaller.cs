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
            serviceCollection.AddTransient<IQuestionApiClient, QuestionApiClient>(provider =>
            {
                var client = CreateApiHttpClient(provider, apiBaseUrl);
                return new QuestionApiClient(client, apiBaseUrl);
            });

            serviceCollection.AddTransient<IResponseApiClient, ResponseApiClient>(provider =>
            {
                var client = CreateApiHttpClient(provider, apiBaseUrl);
                return new ResponseApiClient(client, apiBaseUrl);
            });
            
            serviceCollection.AddTransient<IUserApiClient, UserApiClient>(provider =>
            {
                var client = CreateApiHttpClient(provider, apiBaseUrl);
                return new UserApiClient(client, apiBaseUrl);
            });
            
            serviceCollection.AddTransient<IFormApiClient, FormApiClient>(provider =>
            {
                var client = CreateApiHttpClient(provider, apiBaseUrl);
                return new FormApiClient(client, apiBaseUrl);
            });
            
            serviceCollection.AddTransient<ISearchApiClient, SearchApiClient>(provider =>
            {
                var client = CreateApiHttpClient(provider, apiBaseUrl);
                return new SearchApiClient(client);
            });

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

        public HttpClient CreateApiHttpClient(IServiceProvider serviceProvider, string apiBaseUrl)
        {
            var client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
            client.BaseAddress = new Uri(apiBaseUrl);
            return client;
        }
    }
}