using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;
using Forms.Common.Models.Form;
using Forms.Common.Models.Response;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Forms.Web.App.Pages
{
    public partial class FormDetailPage
    {
        [Inject]
        private FormFacade FormFacade { get; set; } = null!;

        //[Inject]
        //private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Parameter]
        public Guid Id { get; set; }

        public FormDetailModel Data { get; set; } = null!;
        private FormDetailModel? Form { get; set; }
        
        private Guid CurrentUserId { get; set; }
        public EventCallback OnModification { get; set; }

        protected override async Task OnInitializedAsync()
        {
            /*
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("Uživatel není přihlášen.");
            }

            var userIdClaim = user.FindFirst("Id");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("Nelze načíst ID uživatele.");
            }
            CurrentUserId = Guid.Parse(userIdClaim.Value);
            */

            // Inicializace dat formuláře
            Form = await FormFacade.GetByIdAsync(Id);
            // Data = Form ?? GetNewFormModel(CurrentUserId);
            
            // Inicializácia UserResponses pre každú otázku
            if (Form?.Questions != null)
            {
                foreach (var question in Form.Questions)
                {
                    if (!UserResponses.ContainsKey(question.Id))
                    {
                        UserResponses[question.Id] = question.QuestionType == QuestionType.OpenQuestion
                            ? new List<string> { string.Empty } // Pridanie prázdnej odpovede pre OpenQuestion
                            : new List<string>();
                    }
                }
            }
        }

        public async Task Save()
        {
            await FormFacade.SaveAsync(Data);
            Data = GetNewFormModel(CurrentUserId);
            await NotifyOnModification();
        }

        public async Task Delete()
        {
            await FormFacade.DeleteAsync(Id);
            await NotifyOnModification();
        }
        
        private async Task NotifyOnModification()
        {
            if (OnModification.HasDelegate)
            {
                await OnModification.InvokeAsync(null);
            }
        }

        public async Task SaveResponseAsync(ResponseDetailModel response)
        {
            await FormFacade.SaveResponseAsync(response);
        }
        
        private FormDetailModel GetNewFormModel(Guid userId)
            => new()
            {
                Id = Guid.NewGuid(),
                DateOpen = DateTime.Now,
                DateClose = DateTime.Now,
                Name = string.Empty,
                Description = string.Empty,
                UserId = new Guid(),
            };
    }
}
