using System.Globalization;
using AutoMapper;
using Forms.Api.BL.Facades;
using Forms.Common.BL.Facades;
using Forms.Common.Models.Search;
using Forms.Web.BL.Options;
using Microsoft.Extensions.Options;

namespace Forms.Web.BL.Facades;

public class SearchFacade : IAppFacade
{
    private readonly ISearchApiClient apiClient;

    public SearchFacade(
        ISearchApiClient apiClient,
        IMapper mapper,
        IOptions<LocalDbOptions> localDbOptions)
    {
        this.apiClient = apiClient;
    }

    public async Task<ICollection<SearchResultModel>> SearchAsync(string query)
    {
        return await apiClient.SearchAsync(query, CultureInfo.CurrentCulture.Name);
    }
}