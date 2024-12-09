using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper.Internal;
using Forms.Common.Extensions;
using Forms.Web.App;
using Forms.Web.BL.Extensions;
using Forms.Web.BL.Installers;
using Forms.Web.BL.Options;
using Forms.Web.DAL.Installers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap["role"] = ClaimTypes.Role;

builder.RootComponents.Add<App>("app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.json");

var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl"); ;

builder.Services.AddInstaller<WebDALInstaller>();
builder.Services.AddInstaller<WebBLInstaller>(apiBaseUrl);

builder.Services.AddHttpClient("api", client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler(serviceProvider
    => serviceProvider?.GetService<AuthorizationMessageHandler>()
        ?.ConfigureHandler(
            authorizedUrls: new[] { apiBaseUrl },
            scopes: new[] { "formsapi" }));

builder.Services.AddHttpClient("IdentityServer", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IdentityServer:Authority"]);
});


builder.Services.AddHttpClient("AnonymousApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<HttpClient>(serviceProvider => serviceProvider.GetService<IHttpClientFactory>().CreateClient("api"));

builder.Services.AddAutoMapper(configuration =>
    {
        // This is a temporary fix - should be able to remove this when version 11.0.2 comes out
        // More information here: https://github.com/AutoMapper/AutoMapper/issues/3988
        configuration.Internal().MethodMappingEnabled = false;
    }, typeof(WebBLInstaller));
builder.Services.AddLocalization();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("IdentityServer", options.ProviderOptions);
    var configurationSection = builder.Configuration.GetSection("IdentityServer");
    var authority = configurationSection["Authority"];

    options.ProviderOptions.DefaultScopes.Add("formsapi");
});

builder.Services.Configure<LocalDbOptions>(options =>
{
    options.IsLocalDbEnabled = bool.Parse(builder.Configuration.GetSection(nameof(LocalDbOptions))[nameof(LocalDbOptions.IsLocalDbEnabled)]);
});

var host = builder.Build();

await host.RunAsync();
