using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
}
