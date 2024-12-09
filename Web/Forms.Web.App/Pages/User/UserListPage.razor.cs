using System.Security.Claims;
using Forms.Common;
using Forms.Common.Models.User;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class UserListPage
    {
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;

        private ICollection<UserListModel> Users { get; set; } = new List<UserListModel>();
        
        private bool IsAdmin { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            foreach (var claim in user.Claims)
            {
                Console.WriteLine(claim.Type + ": " + claim.Value);
            }

            IsAdmin = user.Claims.Any(c => c.Type == "role" && c.Value == AppRoles.Admin);
            Console.WriteLine($"IsAdmin: {IsAdmin}");
            
            Users = await UserFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }

        
        private async Task DeleteUser(Guid userId)
        {
            Console.WriteLine($"Deleting user {userId}");
            await UserFacade.DeleteAsync(userId);
            Users = await UserFacade.GetAllAsync();

        }
    }
}