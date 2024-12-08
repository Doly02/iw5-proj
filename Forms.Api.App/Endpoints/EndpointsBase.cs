using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Forms.Common;

namespace Forms.Api.App.Endpoints;

public static class EndpointsBase
{
    public static string? GetUserId(IHttpContextAccessor httpContextAccessor)
    {
        var idClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim?.Value;
    }
    
    public static bool IsAdmin(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.IsInRole(AppRoles.Admin) == true;
    }
}
