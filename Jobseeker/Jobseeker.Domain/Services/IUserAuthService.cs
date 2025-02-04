using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Services;

public interface IUserAuthService
{
    Task<string?> AuthenticateUserAsync(string email, string password);
    Task<bool> RegisterUserAsync(User user, string password);
}
