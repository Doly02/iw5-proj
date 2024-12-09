using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Forms.Web.App.Pages
{
    public class PageBase : ComponentBase
    {
        
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public async Task<Guid?> GetUserId()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var currentUser = authState.User;

            if (!currentUser.Identity?.IsAuthenticated ?? true)
            {
                return null;
            }

            var userIdClaim = currentUser.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return null;
            }
            return userId;
        }
    }
}
