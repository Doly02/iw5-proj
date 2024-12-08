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
            async Task<Results<Created<Guid>, BadRequest, BadRequest<string>>> (
                    IAppUserFacade appUserFacade,
                    [FromBody] AppUserCreateModel appUser)
                =>
            {
                try
                {
                    var userId = await appUserFacade.CreateAppUserAsync(appUser);
                    if (userId is not null)
                    {
                        return TypedResults.Created($"/user/{userId.Value}", userId.Value);
                    }

                    return TypedResults.BadRequest();
                }
                catch (ArgumentException e)
                {
                    return TypedResults.BadRequest(e.Message);
                    throw;
                }
            });

        return endpointRouteBuilder;
    }
}
