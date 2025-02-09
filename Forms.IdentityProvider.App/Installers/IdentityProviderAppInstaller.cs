﻿using Forms.Common.Installers;
using Forms.IdentityProvider.DAL;
using Forms.IdentityProvider.DAL.Entities;
using Forms.Web.BL;
using Forms.Web.DAL.Repositories;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Identity;

namespace Forms.IdentityProvider.App.Installers;

public class IdentityProviderAppInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddIdentity<AppUserEntity, AppRoleEntity>()
            .AddEntityFrameworkStores<IdentityProviderDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUserEntity>>(TokenOptions.DefaultProvider);

        serviceCollection.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 5;

            options.SignIn.RequireConfirmedEmail = true;
        });
    
        serviceCollection.AddScoped<IUserApiClient, UserApiClient>();
    }
    
}
