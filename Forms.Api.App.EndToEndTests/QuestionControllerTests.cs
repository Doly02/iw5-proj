using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Forms.Api.DAL.Memory;
using Forms.Common.Enums;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using RichardSzalay.MockHttp;
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
        /* Arrange */
        var storage = new Storage();
        var questionId = storage.Questions[0].Id; 
        var expectedQuestion = new QuestionDetailModel
        {
            Id = questionId,
            Name = "Question 1",
            Description = "Napis",
            QuestionType = QuestionType.Selection,
            Answer = new List<string>(),
            Responses = new List<ResponseDetailModel>()
        };

        var mockHandler = new MockHttpMessageHandler();
        mockHandler.When($"http://localhost/api/Question/{questionId}")
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(expectedQuestion));

        var client = mockHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        /* Act */
        var response = await client.GetAsync($"/api/Question/{questionId}");
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Response content: {content}");

        /* Assert */
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var returnedQuestion = JsonSerializer.Deserialize<QuestionDetailModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(returnedQuestion);

        Assert.Equal(expectedQuestion.Id, returnedQuestion.Id);
        Assert.Equal(expectedQuestion.Name, returnedQuestion.Name);
        Assert.Equal(expectedQuestion.Description, returnedQuestion.Description);
        Assert.Equal(expectedQuestion.Answer, returnedQuestion.Answer);
        Assert.Equal(expectedQuestion.QuestionType, returnedQuestion.QuestionType);
    }

    
    [Fact]
    public async Task Post_Question_Returns_Success()
    {
        var storage = new Storage();
        
        /* Arrange  */
        var questionModel = new QuestionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Open Question",
            Description = "Please write your answer.",
            QuestionType = QuestionType.OpenQuestion,
            Answer = null,  // open question
            FormId = storage.Forms[0].Id,
            Responses = new List<ResponseDetailModel>()  // Initialization of Response List
        };

        /* Serialize The Question Model To JSON Format */
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(questionModel), 
            Encoding.UTF8, 
            "application/json");

        /* Act */
        var serverResponse = await _client.Value.PostAsync("/api/Question", jsonContent);

        /* Assert */
        if (!serverResponse.IsSuccessStatusCode)
        {
            var errorContent = await serverResponse.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {serverResponse.StatusCode}");
            _testOutputHelper.WriteLine($"Response content: {errorContent}");
        }
    
        serverResponse.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

        var responseString = await serverResponse.Content.ReadAsStringAsync();
        var responseGuid = responseString.Trim('"');  // Trim The Response String (GUID Is Enclosed In Quotation Marks)
    
        Assert.Equal(questionModel.Id.ToString(), responseGuid);
    }
    
    
    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

}