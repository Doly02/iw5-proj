using System.Globalization;
using AutoMapper;
using Forms.Api.DAL.EF.Installers;
using Forms.Common;
using Forms.Common.Extensions;
using Forms.Common.Models.User;
using Forms.IdentityProvider.App;
using Forms.IdentityProvider.App.Endpoints;
using Forms.IdentityProvider.App.Installers;
using Forms.IdentityProvider.BL.Installers;
using Forms.IdentityProvider.DAL.Entities;
using Forms.IdentityProvider.DAL.Installers;
using Forms.Web.BL;
using Microsoft.AspNetCore.Identity;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    ConfigureCors(builder.Services);

    builder.Services.AddInstaller<IdentityProviderDALInstaller>();
    builder.Services.AddInstaller<IdentityProviderBLInstaller>();
    builder.Services.AddInstaller<IdentityProviderAppInstaller>();

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var app = builder.ConfigureServices();

    var mapper = app.Services.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();

    app.ConfigurePipeline();

    app.MapGroup("api")
        .AllowAnonymous()
        .UseUserEndpoints();
    
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await SeedRolesAndUsersAsync(serviceProvider);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

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

async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRoleEntity>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUserEntity>>();

    // 1. Vytvoření rolí
    var roles = new[] { AppRoles.Admin, AppRoles.User };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new AppRoleEntity { Name = roleName };
            await roleManager.CreateAsync(role);
        }
    }

    // 2. Vytvoření admin uživatele
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Subject = "admin",
            Email = adminEmail
        };

        var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
    {
        await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
    }

    // 3. Vytvoření normálního uživatele
    var userEmail = "user@example.com";
    var normalUser = await userManager.FindByEmailAsync(userEmail);
    if (normalUser == null)
    {
        normalUser = new AppUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "user",
            Email = userEmail,
            Subject = "user",
        };

        var result = await userManager.CreateAsync(normalUser, "UserPassword123!");
        
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create normal user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    if (!await userManager.IsInRoleAsync(normalUser, AppRoles.User))
    {
        await userManager.AddToRoleAsync(normalUser, AppRoles.User);
    }
    
    // var userApiClient = serviceProvider.GetRequiredService<IUserApiClient>();
    //
    // var adminModel = new UserDetailModel
    // { 
    //     Id = adminUser.Id,
    //     Email = adminUser.Email,
    //     FirstName = "Admin",
    //     LastName = "Adminovic",
    //     PhotoUrl = string.Empty
    // };
    //         
    // await userApiClient.UpsertAsync(CultureInfo.DefaultThreadCurrentCulture?.Name ?? "cs", adminModel);
    //
    // var userModel = new UserDetailModel
    // { 
    //     Id = normalUser.Id,
    //     Email = normalUser.Email,
    //     FirstName = "User",
    //     LastName = "Userovic",
    //     PhotoUrl = string.Empty
    // };
    //         
    // await userApiClient.UpsertAsync(CultureInfo.DefaultThreadCurrentCulture?.Name ?? "cs", userModel);


    
    Console.WriteLine("Seeding completed: Roles and users created.");
}
