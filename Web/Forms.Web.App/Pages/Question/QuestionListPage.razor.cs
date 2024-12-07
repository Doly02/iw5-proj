using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Forms.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Forms.Web.App.Pages
{
    public partial class QuestionListPage
    {
        [Inject]
        private QuestionFacade QuestionFacade { get; set; } = null!;

        [Inject]
        private ResponseFacade ResponseFacade { get; set; } = null!;
        
        [Parameter]
        public Guid FormId { get; set; }

        private ICollection<QuestionListModel> Questions { get; set; } = new List<QuestionListModel>();
        private Dictionary<Guid, List<ResponseDetailModel>> Responses { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Console.WriteLine("Načítám otázky...");
                Questions = await QuestionFacade.GetByFormIdAsync(FormId);
                Console.WriteLine($"Počet načtených otázek: {Questions.Count}");

                foreach (var question in Questions)
                {
                    try
                    {
                        Console.WriteLine($"Načítám odpovědi pro otázku: {question.Name}");
                        var responses = await ResponseFacade.GetByQuestionIdAsync(question.Id);

                        // Zajistíme, že Responses je vždy inicializováno
                        Responses[question.Id] = responses?.ToList() ?? new List<ResponseDetailModel>();
                        Console.WriteLine($"Počet odpovědí pro otázku {question.Name}: {Responses[question.Id].Count}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba při načítání odpovědí pro otázku {question.Name}: {ex.Message}");
                        Responses[question.Id] = new List<ResponseDetailModel>(); // Zajistíme, že šablona může bezpečně pokračovat
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání otázek: {ex.Message}");
            }

            await base.OnInitializedAsync();
        }
    }
}