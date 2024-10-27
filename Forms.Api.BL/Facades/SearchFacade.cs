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

    public async Task<List<SearchResultModel>> SearchAsync(string query)
    {
        var userResults = await _userFacade.SearchAsync(query);
        var questionResults = await _questionFacade.SearchAsync(query);

        return userResults.Concat(questionResults).ToList();
    }
}