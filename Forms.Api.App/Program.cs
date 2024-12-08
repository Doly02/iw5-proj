using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using AutoMapper.Internal;
using Forms.Api.App;
using Forms.Api.App.Endpoints;
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
using Forms.Common;
using Forms.Common.Models.Response;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Common.Models.Search;
using Forms.Common.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
ConfigureAuthentication(builder.Services, builder.Configuration.GetSection("IdentityServer")["Url"]);

var app = builder.Build();

ValidateAutoMapperConfiguration(app.Services);

UseDevelopmentSettings(app);
UseSecurityFeatures(app);
UseLocalization(app);
UseRouting(app);
UseAuthorization(app);
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

void ConfigureAuthentication(IServiceCollection serviceCollection, string identityServerUrl)
{
    serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = identityServerUrl;
            options.TokenValidationParameters.ValidateAudience = false;
        });

    serviceCollection.AddAuthorization(
        options =>
        {
            options.AddPolicy(ApiPolicies.Admin, policy => 
                policy.RequireRole(AppRoles.Admin));
            
            options.AddPolicy(ApiPolicies.OwnerOrAdmin, policy =>
                policy.RequireAssertion(context =>
                {
                    var userId = context.User.FindFirst("sub")?.Value; 
                    var formOwnerId = context.Resource as string; 
                    return userId == formOwnerId || context.User.IsInRole(AppRoles.Admin);
                }));
        });
    serviceCollection.AddHttpContextAccessor();
}


void ValidateAutoMapperConfiguration(IServiceProvider serviceProvider)
{
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

void UseAuthorization(WebApplication application)
{
    application.UseAuthentication();
    application.UseAuthorization();
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

    userEndpoints.MapGet("", (IUserFacade userFacade) => userFacade.GetAll())
        .AllowAnonymous();

    userEndpoints.MapPost("", Results<Ok<Guid>, ForbidHttpResult> (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        try
        {
            return TypedResults.Ok(userFacade.Create(user));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    });

    userEndpoints.MapPost("upsert",  Results<Ok<Guid>, ForbidHttpResult> (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);

        try
        {
            return TypedResults.Ok(userFacade.CreateOrUpdate(user, userId));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    });

    
    userEndpoints.MapGet("{id:guid}", Results<Ok<UserDetailModel>, NotFound<string>> (Guid id, IUserFacade userFacade)
        => userFacade.GetById(id) is { } user
            ? TypedResults.Ok(user)
            : TypedResults.NotFound($"User with ID {id} not found"));
    
    userEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> (UserDetailModel user, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);
        try
        {
            return TypedResults.Ok(userFacade.Update(user, userId));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
        
    });
    
    userEndpoints.MapDelete("{id:guid}", Results<Ok, ForbidHttpResult> (Guid id, IUserFacade userFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        string? userId = null;
        if (!EndpointsBase.IsAdmin(httpContextAccessor))
        {
        userId = EndpointsBase.GetUserId(httpContextAccessor);
        }
        
        try
        {
            userFacade.Delete(id, userId);
            return TypedResults.Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    });
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
    
    var formModifyingEndpoints = formEndpoints.MapGroup("")
            .RequireAuthorization()
        ;

    formModifyingEndpoints.MapPost("", Results<Ok<Guid>, ForbidHttpResult> (FormDetailModel form, IFormFacade formFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Forbid();
        }

        var createdFormId = formFacade.Create(form, userId);
        return TypedResults.Ok(createdFormId);
    });

    formModifyingEndpoints.MapPut("", Results<Ok<Guid?>, ForbidHttpResult> (FormDetailModel form, IFormFacade formFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);
        try
        {
            return TypedResults.Ok(formFacade.Update(form, userId));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    });

    formModifyingEndpoints.MapPost("upsert", Results<Ok<Guid>, ForbidHttpResult> (FormDetailModel form, IFormFacade formFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Forbid();
        }

        var formId = formFacade.CreateOrUpdate(form, userId);
        return TypedResults.Ok(formId);
    });

    formModifyingEndpoints.MapDelete("{id:guid}", Results<NoContent, ForbidHttpResult> (Guid id, IFormFacade formFacade, IHttpContextAccessor httpContextAccessor) =>
    {
        var userId = EndpointsBase.GetUserId(httpContextAccessor);
        try
        {
            formFacade.Delete(id, userId);
            return TypedResults.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    });
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
