using Forms.IdentityProvider.BL.Facades;
using Forms.IdentityProvider.BL.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Forms.IdentityProvider.App.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder UseUserEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var userEndpoints = endpointRouteBuilder.MapGroup("user");

        userEndpoints.MapGet("search",
            async (
                IAppUserFacade appUserFacade,
                string searchString)
                => await appUserFacade.SearchAsync(searchString));

        userEndpoints.MapPost("",
            async Task<IResult> (
                    IAppUserFacade appUserFacade,
                    [FromBody] AppUserCreateModel appUser)
                =>
            {
                try
                {
                    var userId = await appUserFacade.CreateAppUserAsync(appUser);
                    if (userId is not null)
                    {
                        return Results.Created($"{userId.Value}", userId.Value);
                    }

                    return Results.BadRequest("Failed to create user.");
                }
                catch (ArgumentException e)
                {
                    return Results.BadRequest(e.Message);
                }
            });
        
        userEndpoints.MapGet("by-username/{username}",
            async (IAppUserFacade appUserFacade, string username) =>
            {
                var user = await appUserFacade.GetUserByUserNameAsync(username);
                return user is not null
                    ? Results.Ok(user)
                    : Results.NotFound($"User with username '{username}' not found.");
            });

        userEndpoints.MapGet("by-email/{email}",
            async (IAppUserFacade appUserFacade, string email) =>
            {
                var user = await appUserFacade.GetUserByEmailAsync(email);
                return user is not null
                    ? Results.Ok(user)
                    : Results.NotFound($"User with email '{email}' not found.");
            });

        return endpointRouteBuilder;
    }
}
