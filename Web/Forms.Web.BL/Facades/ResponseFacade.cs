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

        public ResponseFacade(
            IResponseApiClient apiClient,
            ResponseRepository responseRepository,
            IMapper mapper,
            IOptions<LocalDbOptions> localDbOptions)
            : base(responseRepository, mapper, localDbOptions)
        {
            this.apiClient = apiClient;
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
    }
}