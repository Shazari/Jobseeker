using FirebaseAdmin.Auth;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Jobseeker.Infrastructure.Services;

public class FirebaseAuthService : IUserAuthService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiKey;

    public FirebaseAuthService(IConfiguration configuration)
    {
        _apiKey = configuration["Firebase:ApiKey"]
                  ?? throw new InvalidOperationException("Firebase API Key is missing in configuration.");
    }

    public async Task<string?> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            if (!user.CustomClaims.ContainsKey("roles"))
            {
                var roles = new List<string> { "Admin", "JobSeeker", "Company" };
                var claims = new Dictionary<string, object>
                {
                    { "roles", roles }
                };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, claims);

                // Re-fetch user to get updated claims
                user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            }

            var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

            var idToken = await ExchangeCustomTokenForIdToken(customToken);
            return idToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            return null;
        }
    }

    private async Task<string?> ExchangeCustomTokenForIdToken(string customToken)
    {
        var payload = new
        {
            token = customToken,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsync(
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={_apiKey}",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to exchange token: {error}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);
        var idToken = document.RootElement.GetProperty("idToken").GetString();

        return idToken;
    }

    public async Task<bool> RegisterUserAsync(User user, string password, List<string> roles)
    {
        try
        {
            var userRecordArgs = new UserRecordArgs
            {
                Email = user.Email,
                Password = password,
                DisplayName = user.FullName,
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

            if (userRecord != null)
            {
                var claims = new Dictionary<string, object>
                {
                    { "roles", string.Join(",", roles) }
                };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"User registration failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateUserRolesAsync(string email, List<string> roles)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            var claims = new Dictionary<string, object>
            {
                { "roles", string.Join(",", roles) }
            };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, claims);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Updating roles failed: {ex.Message}");
            return false;
        }
    }

    public async Task<List<string>> GetUserRolesAsync(string email)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            if (user.CustomClaims.TryGetValue("roles", out var rolesObj) && rolesObj is string rolesStr)
            {
                return rolesStr.Split(',').ToList();
            }
            return new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fetching user roles failed: {ex.Message}");
            return new List<string>();
        }
    }
}