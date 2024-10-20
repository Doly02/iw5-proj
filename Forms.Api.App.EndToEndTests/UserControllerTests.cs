using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Forms.Common.Models.User;
using Xunit;

namespace Forms.Api.App.EndToEndTests
{
    public class UserControllerTests : IAsyncDisposable
    {
        private readonly FormsApiApplicationFactory _app;
        private readonly Lazy<HttpClient> _client;

        public UserControllerTests()
        {
            _app = new FormsApiApplicationFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }

        [Fact]
        public async Task Get_All_Users_Returns_At_Last_One_Recipe()
        {
            var response = await _client.Value.GetAsync("/api/User");

            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<ICollection<UserListModel>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        public async ValueTask DisposeAsync()
        {
            await _app.DisposeAsync();
        }
    }
}