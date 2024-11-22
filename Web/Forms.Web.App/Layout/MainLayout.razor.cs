using Microsoft.AspNetCore.Components;

namespace Forms.Web.App
{
    public partial class MainLayout
    {

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;


        public async Task OnlineStatusChangedAsync(bool isOnline)
        {
            if (isOnline)
            {
                // todo

            }
        }
    }
}
