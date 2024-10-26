using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Forms.Api.DAL.Memory;
using Forms.Common.Models.Search;
using Forms.Common.Models.User;
using Xunit;

namespace Forms.Api.App.EndToEndTests
{
    public class SearchControllerTests : IAsyncDisposable
    {
        private readonly FormsApiApplicationFactory _app;
        private readonly Lazy<HttpClient> _client;

        public SearchControllerTests()
        {
            _app = new FormsApiApplicationFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }


        [Fact]
        public async Task Search_Returns_SearchResultModel_List()
        {
            // Arrange
            var storage = new Storage();
            var query = storage.Users[0].FirstName;
            var expectedUser = storage.Users[0];

            // Act
            var response = await _client.Value.GetAsync($"/api/Search/{query}");

            // Assert
            response.EnsureSuccessStatusCode();  
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();

            var returnedResults = JsonSerializer.Deserialize<List<SearchResultModel>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    
            Assert.NotNull(returnedResults);
            Assert.NotEmpty(returnedResults);

            var matchedResult = returnedResults.FirstOrDefault(r => r.Id == expectedUser.Id);
            Assert.NotNull(matchedResult);
            Assert.Equal(expectedUser.Id, matchedResult.Id);
            Assert.Equal($"{expectedUser.FirstName} {expectedUser.LastName}", matchedResult.Name);
            Assert.Equal(expectedUser.Email, matchedResult.Description);
        }

        [Fact]
        public async Task Search_Returns_Question_SearchResultModel_List()
        {
            // Arrange
            var storage = new Storage();
            var query = storage.Questions[0].Name;
            var expectedQuestion = storage.Questions[0];

            // Act
            var response = await _client.Value.GetAsync($"/api/Search/{query}");

            // Assert
            response.EnsureSuccessStatusCode();  
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();

            var returnedResults = JsonSerializer.Deserialize<List<SearchResultModel>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(returnedResults);
            Assert.NotEmpty(returnedResults);

            var matchedResult = returnedResults.FirstOrDefault(r => r.Id == expectedQuestion.Id);
            Assert.NotNull(matchedResult);
            Assert.Equal(expectedQuestion.Id, matchedResult.Id);
            Assert.Equal(expectedQuestion.Name, matchedResult.Name);
            Assert.Equal(expectedQuestion.Description, matchedResult.Description);
        }


        public async ValueTask DisposeAsync()
        {
            await _app.DisposeAsync();
        }
    }
}