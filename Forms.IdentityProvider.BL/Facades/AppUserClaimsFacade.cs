﻿using AutoMapper;
using Forms.IdentityProvider.BL.Models;
using Forms.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forms.IdentityProvider.BL.Facades;

public class AppUserClaimsFacade : IAppUserClaimsFacade
{
    private readonly UserManager<AppUserEntity> userManager;
    private readonly IMapper mapper;

    public AppUserClaimsFacade(
        UserManager<AppUserEntity> userManager,
        IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<AppUserClaimListModel>> GetAppUserClaimsByUserIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return new List<AppUserClaimListModel>();
        }
        else
        {
            var claims = await userManager.GetClaimsAsync(user);
            return claims.Select(claim => mapper.Map<AppUserClaimListModel>(claim)).ToList();
        }
    }
}
