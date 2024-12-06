using System;
using System.Net.Http;
using Forms.Common.BL.Facades;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.Web.BL.Installers
{
    public class WebBLInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IQuestionApiClient, QuestionApiClient>();

            serviceCollection.AddScoped<IResponseApiClient, ResponseApiClient>();
            
            serviceCollection.AddScoped<IUserApiClient, UserApiClient>();

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<WebBLInstaller>()
                    .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime());
        }

        public HttpClient CreateApiHttpClient(IServiceProvider serviceProvider, string apiBaseUrl)
        {
            var client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
            client.BaseAddress = new Uri(apiBaseUrl);
            return client;
        }
    }
}