using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Forms.Common.Models.User;
using Forms.Api.DAL.Memory;
using Xunit;
using Xunit.Abstractions;

namespace Forms.Api.App.EndToEndTests
{
    public class UserControllerTests : IAsyncDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly FormsApiApplicationFactory _app;
        private readonly Lazy<HttpClient> _client;

        public UserControllerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _app = new FormsApiApplicationFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }

        [Fact]
        public async Task Get_All_Users_Returns_At_Last_One_User()
        {
            var response = await _client.Value.GetAsync("/api/User");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<ICollection<UserListModel>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetById_Returns_UserDetailModel()
        {
            /* Arrange */
            var storage = new Storage();
            var userId = storage.Users[0].Id;
            var expectedUser = storage.Users[0];

            /* Act */
            var response = await _client.Value.GetAsync($"/api/User/{userId}");
            var content = await response.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine($"Response content: {content}");

            /* Assert */
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonSerializer.Deserialize<UserDetailModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(returnedUser);
            
            /* Check of The User's Attributes */
            Assert.Equal(expectedUser.Id, returnedUser.Id);
            Assert.Equal(expectedUser.FirstName, returnedUser.FirstName);
            Assert.Equal(expectedUser.LastName, returnedUser.LastName);
            Assert.Equal(expectedUser.Email, returnedUser.Email);
            Assert.Equal(expectedUser.PhotoUrl, returnedUser.PhotoUrl);
        }
        
        [Fact]
        public async Task Post_User_Returns_Success()
        {
            /* Arrange */
            var userModel = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@example.com",
                PhotoUrl = "https://example.com/photo.jpg"
            };
            
            /* Serialize The UserDetailModel Into JSON Format */
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(userModel), 
                Encoding.UTF8, 
                "application/json");

            /* Act */
            var serverResponse = await _client.Value.PostAsync("/api/User", jsonContent);

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
            /* Trim The Response String (GUID - Enclosed in Quotation Marks) */
            var responseGuid = responseString.Trim('"'); 

            Assert.Equal(userModel.Id.ToString(), responseGuid);
        }
        public async ValueTask DisposeAsync()
        {
            await _app.DisposeAsync();
        }
    }
}