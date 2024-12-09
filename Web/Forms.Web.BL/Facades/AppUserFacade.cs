using System.Net.Http.Json;
using Forms.Common.BL.Facades;
using Forms.Common.Models.User;
using Forms.IdentityProvider.BL.Models;

namespace Forms.Web.BL.Facades;

public class AppUserFacade
{
    private readonly HttpClient _httpClient;

    public AppUserFacade(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Console.WriteLine($"HttpClient BaseAddress: {_httpClient.BaseAddress}");
    }

    public async Task<List<AppUserListModel>?> SearchAsync(string searchString)
    {
        var response = await _httpClient.GetAsync($"user/search?searchString={Uri.EscapeDataString(searchString)}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<AppUserListModel>>();
    }

    public async Task<Guid?> CreateAppUserAsync(AppUserCreateModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("user", user);
        

        if (response.IsSuccessStatusCode)
        {
            // Přečtení ID z těla odpovědi
            var userId = await response.Content.ReadFromJsonAsync<Guid>();
            Console.WriteLine($"User ID from response body: {userId}");
            return userId;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // Přečtení chybové zprávy při neúspěchu
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
            throw new InvalidOperationException($"Server returned error: {error}");
        }

        throw new Exception($"Unexpected status code: {response.StatusCode}");
    }

    
    public async Task<AppUserDetailModel?> GetUserByUserNameAsync(string username)
    {
        var response = await _httpClient.GetAsync($"user/by-username/{Uri.EscapeDataString(username)}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AppUserDetailModel>();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"User with username '{username}' not found.");
            return null;
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Server returned error: {error}");
    }

    public async Task<AppUserDetailModel?> GetUserByEmailAsync(string email)
    {
        var response = await _httpClient.GetAsync($"user/by-email/{Uri.EscapeDataString(email)}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AppUserDetailModel>();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"User with email '{email}' not found.");
            return null;
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Server returned error: {error}");
    }
}