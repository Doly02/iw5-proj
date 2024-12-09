using AutoMapper;
using Forms.Web.BL.Options;
using Forms.Web.DAL.Repositories;
using Forms.Common.Models.Response;
using Microsoft.Extensions.Options;

namespace Forms.Web.BL.Facades
{
    public class ResponseFacade : FacadeBase<ResponseDetailModel, ResponseListModel>
    {
        private readonly IResponseApiClient apiClient;
        private readonly IMapper mapper;

        public ResponseFacade(
            IResponseApiClient apiClient,
            ResponseRepository responseRepository,
            IMapper mapper,
            IOptions<LocalDbOptions> localDbOptions)
            : base(responseRepository, mapper, localDbOptions)
        {
            this.apiClient = apiClient;
            this.mapper = mapper;
        }

        public override async Task<List<ResponseListModel>> GetAllAsync()
        {
            var responsesAll = await base.GetAllAsync();

            var responseFromApi = await apiClient.ResponseGetAsync(culture);
            responsesAll.AddRange(responseFromApi);

            return responsesAll;
        }

        public override async Task<ResponseDetailModel> GetByIdAsync(Guid id)
        {
            return await apiClient.ResponseGetAsync(id, culture);
        }

        protected override async Task<Guid> SaveToApiAsync(ResponseDetailModel data)
        {
            return await apiClient.UpsertAsync(apiVersion, data);
        }

        public override async Task DeleteAsync(Guid id)
        {
            await apiClient.ResponseDeleteAsync(id, culture);
        }
        public async Task<ICollection<ResponseDetailModel>> GetByQuestionIdAsync(Guid questionId)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture.Name; // Nastavení aktuální kultury
            var responsesFromApi = await apiClient.ResponseGetByQuestionIdAsync(questionId, culture);

            if (responsesFromApi == null)
            {
                throw new NullReferenceException($"API did not return responses for questionId: {questionId}");
            }

            return mapper.Map<ICollection<ResponseDetailModel>>(responsesFromApi);
        }
    }
}