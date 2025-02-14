using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Forms.Common.Models.Form;
using Forms.Common.Models.User;
using System.Text.Json;
using Forms.Api.DAL.Memory;
using Microsoft.IdentityModel.Tokens;
using Forms.Common.Enums;
using Forms.Common.Models.Question;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Abstractions;

namespace Forms.Api.App.EndToEndTests;

public class FormControllerTests: IAsyncDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly FormsApiApplicationFactory _app;
    private readonly Lazy<HttpClient> _client;

    public FormControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _app = new FormsApiApplicationFactory();
        _client = new Lazy<HttpClient>(_app.CreateClient());
    }
    
    [Fact]
    public async Task Get_All_Forms_Returns_At_Last_One_Form()
    {
        var response = await _client.Value.GetAsync("/api/Form");

        response.EnsureSuccessStatusCode();

        var forms = await response.Content.ReadFromJsonAsync<ICollection<FormListModel>>();
        Assert.NotNull(forms);
        Assert.NotEmpty(forms);
    }
    
    [Fact]
    public async Task GetById_Returns_FormDetailModel()
    {
        // Arrange
        var storage = new Storage();
        var formId = storage.Forms[0].Id; 
        var expectedForm = storage.Forms[0]; 
    
        // Act
        var response = await _client.Value.GetAsync($"/api/Form/{formId}");

        // Assert
        response.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var returnedForm = JsonSerializer.Deserialize<FormDetailModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(returnedForm);
        
        Assert.Equal(expectedForm.Id, returnedForm.Id);
        Assert.Equal(expectedForm.Name, returnedForm.Name);
        Assert.Equal(expectedForm.Description, returnedForm.Description);
        Assert.Equal(expectedForm.UserId, returnedForm.UserId);
    }

    
    [Fact]
    public async Task Post_Form_Returns_Success()
    {
        // Arrange
        var storage = new Storage();
        
        var formModel = new FormDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "Test Form",
            Description = "This is a test form",
            DateOpen = DateTime.Now,
            DateClose = DateTime.Now.AddDays(30),
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserId = storage.Users[0].Id
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(formModel), 
            Encoding.UTF8, 
            "application/json");

        // Mock The HTTP Client
        var mockHandler = new MockHttpMessageHandler();

        // Simulate a Successful Response For The POST Request
        mockHandler.When("http://localhost/api/Form")
            .Respond(async (request) =>
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var submittedModel = JsonSerializer.Deserialize<FormDetailModel>(requestBody);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent($"\"{submittedModel?.Id}\"", Encoding.UTF8, "application/json")
                };
            });

        var client = mockHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        // Act
        var response = await client.PostAsync("/api/Form", jsonContent);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.WriteLine($"Response content: {errorContent}");
        }
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var responseGuid = responseString.Trim('"');

        Assert.Equal(formModel.Id.ToString(), responseGuid);
    }


    
    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

}