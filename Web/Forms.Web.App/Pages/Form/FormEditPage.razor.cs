using Forms.Common.Enums;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class FormEditPage
    {
        [Parameter]
        public Guid FormId { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private FormFacade FormFacade { get; set; } = null!;

        private FormDetailModel? Form { get; set; }

        private string? NewAnswer { get; set; } = string.Empty;
        private string? ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Form = await FormFacade.GetByIdAsync(FormId);
                if (Form == null)
                {
                    ErrorMessage = "Formulář nebyl nalezen.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Chyba při načítání formuláře: {ex.Message}";
            }
        }

        private async Task UpdateForm()
        {
            try
            {
                if (Form != null)
                {
                    await FormFacade.SaveAsync(Form);
                    NavigationManager.NavigateTo("/forms");
                }
                else
                {
                    ErrorMessage = "Formulář nelze uložit, protože nebyl správně načten.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Chyba při ukládání formuláře: {ex.Message}";
            }
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

        private void AddQuestion()
        {
            if (Form != null)
            {
                Form.Questions.Add(new QuestionDetailModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Nová otázka",
                    Description = "Zadejte popis otázky",
                    QuestionType = QuestionType.OpenQuestion,
                    Answer = new List<string>(),
                    FormId = Form.Id
                });
                ErrorMessage = null;
            }
        }
    }
}
