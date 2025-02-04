using FirebaseAdmin.Auth;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Services;

namespace Jobseeker.Infrastructure.Services;

public class FirebaseAuthService : IUserAuthService
{
    public async Task<string?> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            return user.Uid;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RegisterUserAsync(User user, string password)
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
            return userRecord != null;
        }
        catch
        {
            return false;
        }
    }
}
