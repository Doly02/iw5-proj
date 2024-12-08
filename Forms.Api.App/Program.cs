using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using AutoMapper.Internal;
using Forms.Api.App.Processors;
using Forms.Api.BL.Facades;
using Forms.Api.BL.Installers;
using Forms.Api.DAL.Common;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.EF;
using Forms.Api.DAL.EF.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Localization;
using Forms.Api.DAL.EF.Extensions;
using Forms.Common.Extensions;
using Forms.Api.DAL.Memory.Installers;
using Forms.Common.Models.Response;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Common.Models.Search;
using Forms.Common.Models.User;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FormsDbContext>();
    
    dbContext.Database.Migrate();
    dbContext.SeedData();
}

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
     serviceCollection.AddOpenApiDocument(settings =>
         settings.OperationProcessors.Add(new RequestCultureOperationProcessor()));
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
    UseUserEndpoints(endpointsBase);
    UseResponseEndpoints(endpointsBase);
    UseFormEndpoints(endpointsBase);
    UseQuestionEndpoints(endpointsBase);
    UseSearchEndpoints(endpointsBase);
}

void UseUserEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var userEndpoints = routeGroupBuilder.MapGroup("user")
        .WithTags("user");

    userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll());

    userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
        => userFacade.GetById(id) is { } user
            ? TypedResults.Ok(user)
            : TypedResults.NotFound($"User with ID {id} not found"));

    userEndpoints.MapPost("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Create(user));

    userEndpoints.MapPut("", (UserDetailModel user, IUserFacade userFacade) => userFacade.Update(user));

    userEndpoints.MapPost("upsert", (UserDetailModel user, IUserFacade userFacade) => userFacade.CreateOrUpdate(user));

    userEndpoints.MapDelete("{id:guid}", (Guid id, IUserFacade userFacade) => userFacade.Delete(id));
}

void UseSearchEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var searchEndpoints = routeGroupBuilder.MapGroup("search")
        .WithTags("search");

    searchEndpoints.MapGet("{query}", async (string query, SearchFacade searchFacade) =>
        {
            var results = await searchFacade.SearchAsync(query);

            return results.Any()
                ? Results.Ok(results) 
                : Results.NotFound($"No results found for query '{query}'"); 
        })
        .Produces<List<SearchResultModel>>(StatusCodes.Status200OK) 
        .Produces<string>(StatusCodes.Status404NotFound);
}

void UseFormEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var formEndpoints = routeGroupBuilder.MapGroup("form")
        .WithTags("form");

    formEndpoints.MapGet("", (IFormFacade formFacade) => formFacade.GetAll());

    formEndpoints.MapGet("{id:guid}", Results<Ok<FormDetailModel>, NotFound<string>> (Guid id, IFormFacade formFacade)
        => formFacade.GetById(id) is { } form
            ? TypedResults.Ok(form)
            : TypedResults.NotFound($"Form with ID {id} not found"));

    formEndpoints.MapPost("", (FormDetailModel form, IFormFacade formFacade) => formFacade.Create(form));

    formEndpoints.MapPut("", (FormDetailModel form, IFormFacade formFacade) => formFacade.Update(form));

    formEndpoints.MapPost("upsert", (FormDetailModel form, IFormFacade formFacade) => formFacade.CreateOrUpdate(form));

    formEndpoints.MapDelete("{id:guid}", (Guid id, IFormFacade formFacade) => formFacade.Delete(id));
}

void UseQuestionEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var questionEndpoints = routeGroupBuilder.MapGroup("question")
        .WithTags("question");

    questionEndpoints.MapGet("", (IQuestionFacade questionFacade) => questionFacade.GetAll());

    questionEndpoints.MapGet("{id:guid}", Results<Ok<QuestionDetailModel>, NotFound<string>> (Guid id, IQuestionFacade questionFacade)
        => questionFacade.GetById(id) is { } question
            ? TypedResults.Ok(question)
            : TypedResults.NotFound($"Question with ID {id} not found"));

    questionEndpoints.MapPost("", (QuestionDetailModel question, IQuestionFacade questionFacade) => questionFacade.Create(question));

    questionEndpoints.MapPut("", (QuestionDetailModel question, IQuestionFacade questionFacade) => questionFacade.Update(question));

    questionEndpoints.MapPost("upsert", (QuestionDetailModel question, IQuestionFacade questionFacade) => questionFacade.CreateOrUpdate(question));

    questionEndpoints.MapDelete("{id:guid}", (Guid id, IQuestionFacade questionFacade) => questionFacade.Delete(id));
    
    questionEndpoints.MapGet("form/{formId:guid}", Results<Ok<List<QuestionListModel>>, NotFound<string>> (Guid formId, IQuestionFacade questionFacade)
        => questionFacade.GetByFormId(formId) is { } questions && questions.Any()
            ? TypedResults.Ok(questions)
            : TypedResults.NotFound($"No questions found for FormId {formId}"));
}

void UseResponseEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var responseEndpoints = routeGroupBuilder.MapGroup("response")
        .WithTags("response");

    responseEndpoints.MapGet("", (IResponseFacade responseFacade) => responseFacade.GetAll());

    responseEndpoints.MapGet("{id:guid}", Results<Ok<ResponseDetailModel>, NotFound<string>> (Guid id, IResponseFacade responseFacade)
        => responseFacade.GetById(id) is { } response
            ? TypedResults.Ok(response)
            : TypedResults.NotFound($"Response with ID {id} not found"));

    responseEndpoints.MapPost("", (ResponseDetailModel response, IResponseFacade responseFacade) => responseFacade.Create(response));

    responseEndpoints.MapPut("", (ResponseDetailModel response, IResponseFacade responseFacade) => responseFacade.Update(response));

    responseEndpoints.MapPost("upsert", (ResponseDetailModel response, IResponseFacade responseFacade) => responseFacade.CreateOrUpdate(response));

    responseEndpoints.MapDelete("{id:guid}", (Guid id, IResponseFacade responseFacade) => responseFacade.Delete(id));
    
    responseEndpoints.MapGet("question/{questionId:guid}", Results<Ok<List<ResponseDetailModel>>, NotFound<string>> (Guid questionId, IResponseFacade responseFacade)
        => responseFacade.GetByQuestionId(questionId) is { } responses && responses.Any()
            ? TypedResults.Ok(responses)
            : TypedResults.NotFound($"No responses found for QuestionId {questionId}"));
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
