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

        protected override async Task OnInitializedAsync()
        {
            Users = await UserFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }
    }
}