using System.Net;
using System.Net.Http.Json;
using System.Text;
using Forms.Common.Models.Form;
using Forms.Common.Models.User;
using System.Text.Json;
using Forms.Api.DAL.Memory;
using Forms.Common.Enums;
using Forms.Common.Models.Question;
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

        // Serialize
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(formModel), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.Value.PostAsync("/api/Form", jsonContent);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            // Pokud server vrátí chybu, vypíšeme stavový kód a detail odpovědi
            var errorContent = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Error: {response.StatusCode}");
            _testOutputHelper.WriteLine($"Response content: {errorContent}");
        }
        
        // Assert todo assert also other attributes
        response.EnsureSuccessStatusCode();  
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        
        // response string is in format "GUID", get rid of "
        var responseGuid = responseString.Trim('"');
        
        Assert.Equal(formModel.Id.ToString(), responseGuid);
    }

    
    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }

}