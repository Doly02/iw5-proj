using System.Security.Claims;
using Forms.IdentityProvider.BL.Facades;
using Forms.IdentityProvider.BL.Models;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;

namespace Forms.IdentityProvider.App.Services;

public class LocalAppUserProfileService : IProfileService
{
    private readonly IAppUserFacade appUserFacade;
    private readonly IAppUserClaimsFacade appUserClaimsFacade;

    public LocalAppUserProfileService(
        IAppUserFacade appUserFacade,
        IAppUserClaimsFacade appUserClaimsFacade)
    {
        this.appUserFacade = appUserFacade;
        this.appUserClaimsFacade = appUserClaimsFacade;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        Console.WriteLine($"Getting user info {subjectId}");

        AppUserDetailModel? user;

        if(Guid.TryParse(subjectId, out var id))
        {
            user = await appUserFacade.GetUserByIdAsync(id);
        }
        else
        {
            user = await appUserFacade.GetUserByUserNameAsync(subjectId);
        }

        if(user is not null)
        {
            if (user is not null)
            {
                var appUserClaims = await appUserClaimsFacade.GetAppUserClaimsByUserIdAsync(user.Id);
                var claims = appUserClaims.Select(claim =>
                {
                    if (claim.ClaimType is not null
                        && claim.ClaimValue is not null)
                    {
                        return new Claim(claim.ClaimType, claim.ClaimValue);
                    }
                    return null;
                }).ToList();

                claims.Add(new Claim("username", user.UserName));
                claims.Add(new Claim("id", user.Id.ToString()));
                
                var userRoles = await appUserFacade.GetUserRolesByIdAsync(user.Id);
                Console.WriteLine($"Roles for user {user.UserName}: {string.Join(", ", userRoles)}");
                foreach (var role in userRoles)
                {
                    Console.WriteLine($"adding role {role}");
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }
                
                context.AddRequestedClaims(claims);
                
                Console.WriteLine("Claims added to token:");

            }
        }
        Console.WriteLine("Getting user infooooo");

    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
    }
}
