using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Forms.Api.DAL.Memory;
using Forms.Common.Models.Form;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Forms.Common.Models.User;
using Xunit;
using Xunit.Abstractions;

namespace Forms.Api.App.EndToEndTests;

public class ResponseControllerTests: IAsyncDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly FormsApiApplicationFactory _app;
    private readonly Lazy<HttpClient> _client;

    public ResponseControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _app = new FormsApiApplicationFactory();
        _client = new Lazy<HttpClient>(_app.CreateClient());
    }
    
    [Fact]
    public async Task Get_All_Responses_Returns_At_Last_One_Response()
    {
        var response = await _client.Value.GetAsync("/api/Response");

        response.EnsureSuccessStatusCode();

        var responses = await response.Content.ReadFromJsonAsync<ICollection<ResponseListModel>>();
        Assert.NotNull(responses);
        Assert.NotEmpty(responses);
    }
    
    [Fact]
    public async Task GetById_Returns_ResponseDetailModel()
    {
        // Arrange
        var storage = new Storage();
        var responseId = storage.Responses[0].Id; 
        var expectedResponse = storage.Responses[0]; 
    
        // Act
        var response = await _client.Value.GetAsync($"/api/Response/{responseId}");

        // Assert
        response.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var returnedResponse = JsonSerializer.Deserialize<ResponseDetailModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(returnedResponse);
        
        Assert.Equal(expectedResponse.Id, returnedResponse.Id);
        // todo w8 for qlistmodel
        // Assert.Equal(expectedResponse.Question.Id, returnedResponse.Question.Id);
        Assert.Equal(expectedResponse.User.Id, returnedResponse.User.Id);
        Assert.Equal(expectedResponse.UserResponse, returnedResponse.UserResponse);
    }

    
    [Fact]
    public async Task Post_Response_Returns_Success()
    {
        // Arrange
        var storage = new Storage();

        var responseModel = new ResponseDetailModel
        {
            Id = Guid.NewGuid(),
            Question = new QuestionListModel
            {
                Id = storage.Questions[0].Id,
                Name = storage.Questions[0].Name,
                Description = storage.Questions[0].Description,
                Answer = storage.Questions[0].Answer,
                QuestionType = storage.Questions[0].QuestionType,
            },
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserResponse = new List<string> { "testCreatedResponse" }
        };

        // Serialize
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(responseModel), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.Value.PostAsync("/api/Response", jsonContent);
        
        // Assert
        response.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        
        // response string is in format "GUID", get rid of "
        var responseGuid = responseString.Trim('"');
        
        Assert.Equal(responseModel.Id.ToString(), responseGuid);
    }

    
    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

}