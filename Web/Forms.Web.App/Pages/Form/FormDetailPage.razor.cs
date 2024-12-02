using Forms.Common.Models.Form;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class FormDetailPage
    {
        [Inject]
        private FormFacade FormFacade { get; set; } = null!;

        [Parameter]
        public Guid Id { get; set; }

        private FormDetailModel? Form { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Form = await FormFacade.GetByIdAsync(Id);
        }
    }
}