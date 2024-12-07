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
            
            serviceCollection.AddScoped<IUserApiClient, UserApiClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = clientFactory.CreateClient("AnonymousApi"); 
                return new UserApiClient(client);
            });

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<WebBLInstaller>()
                    .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime());
        }
    }
}