using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Interfaces;

public interface IJobPostRepository : IRepository<JobPost>
{
    Task<IList<JobPost>?> GetByCompanyAsync(string companyName);
}
