using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Interfaces;

public interface IJobApplicationRepository : IRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetByUserIdAsync(Guid userId);
}
