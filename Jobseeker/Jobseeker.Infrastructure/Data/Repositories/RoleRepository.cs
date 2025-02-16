using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Interfaces;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext databaseContext) : base(databaseContext)
    {
        
    }
}
