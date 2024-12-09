using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Forms.Common.Models.User;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Forms.Web.App.Pages
{
    public partial class FormDetailPage
    {
        [Inject]
        private FormFacade FormFacade { get; set; } = null!;
        [Inject]
        private QuestionFacade QuestionFacade { get; set; } = null!;
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public Guid Id { get; set; }

        public FormDetailModel Data { get; set; } = null!;
        private FormDetailModel? Form { get; set; }
        private readonly string culture = "en-US";
        private Guid CurrentUserId { get; set; }
        private UserDetailModel? CurrentUser { get; set; }
        public EventCallback OnModification { get; set; }

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

            // Načítanie používateľa z UserFacade
            CurrentUser = await UserFacade.GetByIdAsync(CurrentUserId) 
                          ?? throw new InvalidOperationException("Používateľ neexistuje.");

            // Načítanie formulára
            Form = await FormFacade.GetByIdAsync(Id);
            Data = Form ?? GetNewFormModel(CurrentUserId);

            // Inicializácia UserResponses pre každú otázku
            if (Form?.Questions != null)
            {
                foreach (var question in Form.Questions)
                {
                    if (!UserResponses.ContainsKey(question.Id))
                    {
                        UserResponses[question.Id] = question.QuestionType == QuestionType.OpenQuestion
                            ? new List<string> { string.Empty }
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
            await FormFacade.SaveResponseAsync(response, culture);
        }
        
        private FormDetailModel GetNewFormModel(Guid userId)
            => new()
            {
                Id = Guid.NewGuid(),
                DateOpen = DateTime.Now,
                DateClose = DateTime.Now,
                Name = string.Empty,
                Description = string.Empty,
                UserId = userId,
            };
        
        private Dictionary<Guid, List<string>> UserResponses { get; set; } = new();

        private void UpdateCheckboxResponse(QuestionDetailModel question, string answer, ChangeEventArgs e)
        {
            Console.WriteLine($"Zpracovávám odpověď: {answer}, Stav: {e.Value}");
        
            // Initialize The List of Answers For Question
            if (!UserResponses.ContainsKey(question.Id))
            {
                UserResponses[question.Id] = new List<string>();
            }

            // Validation of Checkbox's Actual State
            var isChecked = e.Value?.ToString()!.ToLower() == "true";

            if (isChecked)
            {
                // If Checkbox Is Checked, Add Answer If It's Not In UserResponse
                if (!UserResponses[question.Id].Contains(answer))
                {
                    UserResponses[question.Id].Add(answer);
                    Console.WriteLine($"Přidávám odpověď '{answer}' pro otázku {question.Name}.");
                }
                else
                {
                    Console.WriteLine($"Odpověď '{answer}' již existuje pro otázku {question.Name}.");
                }
            }
            else
            {
                // If Checkbox Is Not Checked, Removed Answer If Exists
                if (UserResponses[question.Id].Contains(answer))
                {
                    UserResponses[question.Id].Remove(answer);
                    Console.WriteLine($"Odebírám odpověď '{answer}' pro otázku {question.Name}.");
                }
                else
                {
                    Console.WriteLine($"Odpověď '{answer}' neexistuje pro otázku {question.Name}, nic se neodstraní.");
                }
            }
            Console.WriteLine($"Aktualizované odpovědi pro otázku {question.Name}: {string.Join(", ", UserResponses[question.Id])}");
        }
            
        public async Task SubmitResponses()
        {
            if (Form?.Questions == null)
                return;

            foreach (var question in Form.Questions)
            {
                if (UserResponses.TryGetValue(question.Id, out var responses) && responses.Any())
                {
                    // Načítanie otázky pomocou QuestionFacade
                    var questionDetail = await QuestionFacade.GetByIdAsync(question.Id);
                    if (questionDetail == null)
                    {
                        Console.WriteLine($"Otázka s ID {question.Id} neexistuje.");
                        continue;
                    }

                    // Vytvorenie ResponseDetailModel
                    if (CurrentUser != null)
                    {
                        var response = new ResponseDetailModel
                        {
                            Id = Guid.NewGuid(),
                            User = CurrentUser, // Použitie aktuálneho používateľa
                            Question = new QuestionListModel
                            {
                                Id = questionDetail.Id,
                                Name = questionDetail.Name,
                                Description = questionDetail.Description,
                                QuestionType = questionDetail.QuestionType,
                                Answer = questionDetail.Answer
                            },
                            UserResponse = new List<string>(responses)
                        };

                        // Uloženie odpovede
                        await SaveResponseAsync(response);
                        NavigationManager.NavigateTo("/forms");
                    }
                }
            }

            Console.WriteLine("Všechny odpovědi byly úspěšně odeslány.");
        }
        private void UpdateRadioResponse(QuestionDetailModel question, string selectedAnswer)
        {
            // Ensure that the question's responses are initialized
            if (!UserResponses.ContainsKey(question.Id))
            {
                UserResponses[question.Id] = new List<string>();
            }

            // Clear previous response and add the new one
            UserResponses[question.Id].Clear();
            UserResponses[question.Id].Add(selectedAnswer);

            Console.WriteLine($"Selected answer for question {question.Name}: {selectedAnswer}");
        }
        
    }
}
