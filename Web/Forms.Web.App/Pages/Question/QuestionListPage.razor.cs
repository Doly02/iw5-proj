using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class QuestionListPage
    {
        [Inject]
        private QuestionFacade QuestionFacade { get; set; } = null!;

        private ICollection<QuestionListModel> Questions { get; set; } = new List<QuestionListModel>();

        protected override async Task OnInitializedAsync()
        {
            Questions = await QuestionFacade.GetAllAsync();

            await base.OnInitializedAsync();
        }
    }
}