using Forms.Common.Models.Form;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class FormCreatePage
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private FormFacade FormFacade { get; set; } = null!;

        private FormDetailModel Form { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Description = string.Empty,
            DateOpen = DateTime.Now,
            DateClose = DateTime.Now.AddDays(7),
            Questions = new List<Forms.Common.Models.Question.QuestionDetailModel>(),
            UserId = Guid.Parse("b3f62b8b-291c-42e5-b8d9-3afbb3276dca")   //TODO upravit podla aktualne prihlaseneho usera

        };
        
        private string? ErrorMessage { get; set; }

        private async Task SaveForm()
        {
            try
            {
                await FormFacade.SaveAsync(Form);
                NavigationManager.NavigateTo("/forms");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Chyba pri ukladaní formulára.";
            }
        }

    }
}