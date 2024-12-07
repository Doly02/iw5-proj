using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Web.BL.Options;
using Forms.Web.DAL.Repositories;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Forms.Common.Models.User;
using Microsoft.Extensions.Options;

namespace Forms.Web.BL.Facades;

public class FormFacade : FacadeBase<FormDetailModel, FormListModel>
{
    private readonly IFormApiClient _apiClient;

    private readonly ResponseRepository _responseRepository;
    public FormFacade(
        IFormApiClient apiClient,
        FormRepository formRepository,
        ResponseRepository responseRepository,
        IMapper mapper,
        IOptions<LocalDbOptions> localDbOptions)
        : base(formRepository, mapper, localDbOptions)
    {
        this._apiClient = apiClient;
        _responseRepository = responseRepository;
    }
    
    public override async Task<List<FormListModel>> GetAllAsync()
    {
        var formsAll = await base.GetAllAsync();

        var formsFromApi = await _apiClient.FormGetAsync(culture);
        foreach (var formFromApi in formsFromApi)
        {
            if (formsAll.Any(r => r.Id == formFromApi.Id) is false)
            {
                formsAll.Add(formFromApi);
            }
        }

        return formsAll;
    }

    public override async Task<FormDetailModel> GetByIdAsync(Guid id)
    {
        return await _apiClient.FormGetAsync(id, culture);
    }

    protected override async Task<Guid> SaveToApiAsync(FormDetailModel data)
    {
        return await _apiClient.UpsertAsync(culture, data);
    }

    public async Task SaveResponseAsync(ResponseDetailModel response)
    {
        await _responseRepository.InsertAsync(response);
    }
    public override async Task DeleteAsync(Guid id)
    {
        await _apiClient.FormDeleteAsync(id, culture);
    }
}