using Forms.Common.Models.Response;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class ResponseListPage
    {
        [Inject]
        private ResponseFacade ResponseFacade { get; set; } = null!;

        private ICollection<ResponseListModel> Responses { get; set; } = new List<ResponseListModel>();

        protected override async Task OnInitializedAsync()
        {
            Responses = await ResponseFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }
    }
}