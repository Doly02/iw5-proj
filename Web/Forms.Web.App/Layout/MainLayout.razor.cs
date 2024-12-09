using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App
{
    public partial class MainLayout
    {
        [Inject]
        public UserFacade UserFacade { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        private bool IsOnline { get; set; } = true; 

        public async Task OnlineStatusChangedAsync(bool isOnline)
        {
            Console.WriteLine($"Online status changed: {isOnline}");
            IsOnline = isOnline;

            if (isOnline)
            {
                var dataChanged = false;

                try
                {
                    // Synchronizace dat
                    dataChanged = await UserFacade.SynchronizeLocalDataAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Synchronization failed: {ex.Message}");
                }

                // Pokud byla data změněna, obnovíme aktuální stránku
                if (dataChanged)
                {
                    NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                }
            }

            // Zajistíme aktualizaci stavu uživatelského rozhraní
            InvokeAsync(StateHasChanged);
        }
    }
}