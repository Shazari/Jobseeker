using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    internal UserRepository(ApplicationDbContext databaseContext) : base(databaseContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var result =
            await DbSet
            .Where(user => user.Email != null 
                                && user.Email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync();

        return result;
    }
}
