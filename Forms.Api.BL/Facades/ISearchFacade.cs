using Forms.Common.Models.Search;

namespace Forms.Api.BL.Facades;

public interface ISearchFacade
{
    Task<List<SearchResultModel>> SearchAsync(string query);
}