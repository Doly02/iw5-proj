using Forms.Common.Models.Form;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class FormListPage
    {
        [Inject]
        private FormFacade FormFacade { get; set; } = null!;

        private ICollection<FormListModel> Forms { get; set; } = new List<FormListModel>();

        protected override async Task OnInitializedAsync()
        {
            Forms = await FormFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }
    }
}