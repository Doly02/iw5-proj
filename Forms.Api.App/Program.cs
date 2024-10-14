using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using AutoMapper.Internal;
using Forms.Api.BL.Installers;
using Forms.Api.DAL.Common;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.EF.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Localization;
using Forms.Api.DAL.EF.Extensions;
using Forms.Common.Extensions;
using Forms.Api.DAL.Memory.Installers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder();

ConfigureCors(builder.Services);
ConfigureLocalization(builder.Services);

ConfigureOpenApiDocuments(builder.Services);
ConfigureDependencies(builder.Services, builder.Configuration);
ConfigureAutoMapper(builder.Services);

var app = builder.Build();

ValidateAutoMapperConfiguration(app.Services);

UseDevelopmentSettings(app);
UseSecurityFeatures(app);
UseLocalization(app);
UseRouting(app);
UseEndpoints(app);
UseOpenApi(app);

app.Run();

void ConfigureCors(IServiceCollection serviceCollection)
{
    serviceCollection.AddCors(options =>
    {
        options.AddDefaultPolicy(o =>
            o.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

void ConfigureLocalization(IServiceCollection serviceCollection)
{
    serviceCollection.AddLocalization(options => options.ResourcesPath = string.Empty);
}

void ConfigureOpenApiDocuments(IServiceCollection serviceCollection)
{
    serviceCollection.AddEndpointsApiExplorer();
    // serviceCollection.AddOpenApiDocument(settings =>
    //     settings.OperationProcessors.Add(new RequestCultureOperationProcessor()));
}

void ConfigureDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
{
    if (!Enum.TryParse<DALType>(configuration.GetSection("DALSelectionOptions")["Type"], out var dalType))
    {
        throw new ArgumentException("DALSelectionOptions:Type");
    }

    switch (dalType)
    {
        case DALType.Memory:
            serviceCollection.AddInstaller<ApiDALMemoryInstaller>();
            break;
        case DALType.EntityFramework:
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("The connection string is missing");
            serviceCollection.AddInstaller<ApiDALEFInstaller>(connectionString);
            break;
    }

    serviceCollection.AddInstaller<ApiBLInstaller>();
}

void ConfigureAutoMapper(IServiceCollection serviceCollection)
{
    serviceCollection.AddAutoMapper(typeof(EntityBase), typeof(ApiBLInstaller));
}

void ValidateAutoMapperConfiguration(IServiceProvider serviceProvider)
{
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

void UseEndpoints(WebApplication application)
{
    var endpointsBase = application.MapGroup("api")
        .WithOpenApi();

    // Use***Endpoints(endpointsBase);
}

void UseDevelopmentSettings(WebApplication application)
{
    var environment = application.Services.GetRequiredService<IWebHostEnvironment>();

    if (environment.IsDevelopment())
    {
        application.UseDeveloperExceptionPage();
    }
}

void UseSecurityFeatures(IApplicationBuilder application)
{
    application.UseCors();
    application.UseHttpsRedirection();
}

void UseLocalization(IApplicationBuilder application)
{
    application.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(new CultureInfo("en")),
        SupportedCultures = new List<CultureInfo> { new("en"), new("cs") }
    });

    // application.UseRequestCulture();
}

void UseRouting(IApplicationBuilder application)
{
    application.UseRouting();
}

void UseOpenApi(IApplicationBuilder application)
{
    application.UseOpenApi();
    application.UseSwaggerUi();
}


// Make the implicit Program class public so test projects can access it
public partial class Program
{
}
