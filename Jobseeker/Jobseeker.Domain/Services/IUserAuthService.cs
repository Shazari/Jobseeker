using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Services;

public interface IUserAuthService
{
    Task<string?> AuthenticateUserAsync(string email, string password);
    Task<bool> RegisterUserAsync(User user, string password, List<string> roles);
    Task<bool> UpdateUserRolesAsync(string email, List<string> roles);
    Task<List<string>> GetUserRolesAsync(string email);

}
