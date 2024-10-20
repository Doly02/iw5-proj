using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Forms.Api.App.EndToEndTests;

public class FormsApiApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(collection =>
        {
            var controller = typeof(Program).Assembly.FullName;
            if (controller == null)
            {
                throw new InvalidOperationException("Assembly Name For The Program Could Not Be Retrieved.");
            }

            collection.AddMvc().AddApplicationPart(Assembly.Load(controller));
        });
        return base.CreateHost(builder);
    }
}