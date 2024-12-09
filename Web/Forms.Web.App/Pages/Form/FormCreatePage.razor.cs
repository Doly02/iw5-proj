using Forms.Common.Enums;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Forms.Web.App.Pages
{
    public partial class FormCreatePage
    {
        [Inject]
        private new AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private FormFacade FormFacade { get; set; } = null!;
        private string? NewAnswer { get; set; } = string.Empty;
        private string? ErrorMessage { get; set; }
        private static Guid CurrentUserId { get; set; }

        private FormDetailModel? Form { get; set; }
        
        private QuestionDetailModel NewQuestion { get; set; } = new()
        {
            Id = Guid.NewGuid(),
            Name = "Nová otázka",
            Description = "Zadejte popis otázky",
            QuestionType = QuestionType.OpenQuestion,
            Answer = new List<string>()
        };
        
        protected override async Task OnInitializedAsync()
        {
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
            
            Console.WriteLine($"User Id: {CurrentUserId}");
            
            Form = new FormDetailModel
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                Description = string.Empty,
                DateOpen = DateTime.Now,
                DateClose = DateTime.Now.AddDays(7),
                Questions = new List<QuestionDetailModel>(),
                UserId = CurrentUserId
            };
        }

        private async Task SaveForm()
        {
            try
            {
                if (Form != null) await FormFacade.SaveAsync(Form);
                NavigationManager.NavigateTo("/forms");
            }
            catch (Exception)
            {
                ErrorMessage = "Chyba při ukládání formuláře.";
            }
        }

        private void AddQuestion()
        {
            
            if (string.IsNullOrWhiteSpace(NewQuestion.Name))
            {
                ErrorMessage = "Název otázky nesmí být prázdný.";
                return;
            }

            Form?.Questions.Add(new QuestionDetailModel
            {
                Id = Guid.NewGuid(),
                Name = NewQuestion.Name,
                Description = NewQuestion.Description,
                QuestionType = NewQuestion.QuestionType,
                Answer = new List<string>()
            });

            // Reset Question For Addition
            ResetNewQuestion();

            ErrorMessage = null;
        }

        private void AddAnswer(QuestionDetailModel question)
        {
            if (string.IsNullOrWhiteSpace(NewAnswer))
            {
                ErrorMessage = "Odpověď nesmí být prázdná.";
                return;
            }

            question.Answer?.Add(NewAnswer);
            NewAnswer = string.Empty;
            ErrorMessage = null;
        }

        private void RemoveAnswer(QuestionDetailModel question, string answer)
        {
            question.Answer?.Remove(answer);
        }

        private void RemoveQuestion(QuestionDetailModel question)
        {
            Form?.Questions.Remove(question);
        }

        private void ResetNewQuestion()
        {
            NewQuestion = new QuestionDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Nová otázka",
                Description = "Zadejte popis otázky",
                QuestionType = QuestionType.OpenQuestion,
                Answer = new List<string>()
            };
        }
    }
}