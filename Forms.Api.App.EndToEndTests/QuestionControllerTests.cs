using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Forms.Api.DAL.Memory;
using Forms.Common.Enums;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Xunit;
using Xunit.Abstractions;

namespace Forms.Api.App.EndToEndTests;

public class QuestionControllerTests: IAsyncDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly FormsApiApplicationFactory _app;
    private readonly Lazy<HttpClient> _client;

    public QuestionControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _app = new FormsApiApplicationFactory();
        _client = new Lazy<HttpClient>(_app.CreateClient());
    }
    
    [Fact]
    public async Task Get_All_Questions_Returns_At_Last_One_Question()
    {
        var response = await _client.Value.GetAsync("/api/Question");

        response.EnsureSuccessStatusCode();

        var questions = await response.Content.ReadFromJsonAsync<ICollection<QuestionListModel>>();
        Assert.NotNull(questions);
        Assert.NotEmpty(questions);
    }
    
    [Fact]
    public async Task GetById_Returns_QuestionDetailModel()
    {
        // arrange
        var storage = new Storage();
        var questionId = storage.Questions[0].Id; 
        var expectedQuestion = storage.Questions[0]; 
    
        // act
        var response = await _client.Value.GetAsync($"/api/Question/{questionId}");

        // assert
        response.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var returnedQuestion = JsonSerializer.Deserialize<QuestionDetailModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(returnedQuestion);
        
        Assert.Equal(expectedQuestion.Id, returnedQuestion.Id);
        Assert.Equal(expectedQuestion.Name, returnedQuestion.Name);
        Assert.Equal(expectedQuestion.Description, returnedQuestion.Description);
        Assert.Equal(expectedQuestion.Answer, returnedQuestion.Answer);
        Assert.Equal(expectedQuestion.QuestionType, returnedQuestion.QuestionType);
        
        // testing every Response in the list
        Assert.Equal(expectedQuestion.Responses.Count, returnedQuestion.Responses.Count);

        using var expectedEnumerator = expectedQuestion.Responses.GetEnumerator();
        using var returnedEnumerator = returnedQuestion.Responses.GetEnumerator();
        while (expectedEnumerator.MoveNext() && returnedEnumerator.MoveNext())
        {
            var expectedResponse = expectedEnumerator.Current;
            var returnedResponse = returnedEnumerator.Current;
        
            Assert.Equal(expectedResponse.Id, returnedResponse.Id);
            Assert.Equal(expectedResponse.UserId, returnedResponse.UserId);
            Assert.Equal(expectedResponse.QuestionId, returnedResponse.QuestionId);
            Assert.Equal(expectedResponse.UserResponse, returnedResponse.UserResponse!);
        }
    }

    
    [Fact]
    public async Task Post_Question_Returns_Success()
    {
        // arrange
        var questionModel = new QuestionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Open Question",
            Description = "Please write your answer.",
            QuestionType = QuestionType.OpenQuestion,
            Answer = null,  // open question
            Responses = new List<ResponseDetailModel>()  // initialization of Response list
        };

        // serialize the question model to JSON format
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(questionModel), 
            Encoding.UTF8, 
            "application/json");

        // act
        var serverResponse = await _client.Value.PostAsync("/api/Question", jsonContent);

        // assert
        if (!serverResponse.IsSuccessStatusCode)
        {
            // server response string is in format "GUID", get rid of "
            var errorContent = await serverResponse.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {serverResponse.StatusCode}");
            _testOutputHelper.WriteLine($"Response content: {errorContent}");
        }
    
        serverResponse.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

        var responseString = await serverResponse.Content.ReadAsStringAsync();
        var responseGuid = responseString.Trim('"');  // Trim the response string (GUID is enclosed in quotation marks)
    
        Assert.Equal(questionModel.Id.ToString(), responseGuid);
    }
    
    
    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

}