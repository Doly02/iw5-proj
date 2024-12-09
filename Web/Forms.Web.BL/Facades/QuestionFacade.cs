using AutoMapper;
using Forms.Web.BL.Options;
using Forms.Common.BL.Facades;
using Forms.Web.DAL.Repositories;
using Forms.Common.Models.Question;
using Microsoft.Extensions.Options;

namespace Forms.Web.BL.Facades
{
    public class QuestionFacade : FacadeBase<QuestionDetailModel, QuestionListModel>
    {
        private readonly IQuestionApiClient apiClient;
        private readonly IMapper mapper;
        public QuestionFacade(
            IQuestionApiClient apiClient,
            QuestionRepository questionRepository,
            IMapper mapper,
            IOptions<LocalDbOptions> localDbOptions)
            : base(questionRepository, mapper, localDbOptions)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
        }

        public override async Task<List<QuestionListModel>> GetAllAsync()
        {
            var questionsAll = await base.GetAllAsync();

            var questionsFromApi = await apiClient.QuestionGetAsync(culture);
            questionsAll.AddRange(questionsFromApi);

            return questionsAll;
        }

        public override async Task<QuestionDetailModel> GetByIdAsync(Guid id)
        {
            return await apiClient.QuestionGetAsync(id, culture);
        }

        protected override async Task<Guid> SaveToApiAsync(QuestionDetailModel data)
        {
            return await apiClient.UpsertAsync(apiVersion, data);
        }

        public override async Task DeleteAsync(Guid id)
        {
            await apiClient.QuestionDeleteAsync(id, culture);
        }
        
        public async Task<List<QuestionListModel>> GetByFormIdAsync(Guid formId)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture.Name;
            var questionsFromApi = await apiClient.QuestionGetByFormIdAsync(formId, culture);
            return mapper.Map<List<QuestionListModel>>(questionsFromApi);
        }
    }
}