using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Interfaces;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class JobApplicationRepository : Repository<JobApplication>, IJobApplicationRepository
{
    internal JobApplicationRepository(ApplicationDbContext databaseContext) : base(databaseContext)
    {
    }
}
