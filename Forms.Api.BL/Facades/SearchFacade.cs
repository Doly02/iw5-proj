using Forms.Common.Models.Search;

namespace Forms.Api.BL.Facades;

public class SearchFacade : ISearchFacade
{
    private readonly IUserFacade _userFacade;
    private readonly IQuestionFacade _questionFacade;

    public SearchFacade(IUserFacade userFacade, IQuestionFacade questionFacade)
    {
        _userFacade = userFacade;
        _questionFacade = questionFacade;
    }

    public async Task<List<SearchResultModel>> SearchAsync(string searchText)
    {
        var userResults = await _userFacade.SearchAsync(searchText);
        var questionResults = await _questionFacade.SearchAsync(searchText);

        return userResults.Concat(questionResults).ToList();
    }
}