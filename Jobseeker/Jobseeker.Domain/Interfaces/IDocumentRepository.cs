using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Domain.Interfaces;

public interface IDocumentRepository : IRepository<JobSeekerDocument>
{
    Task<IList<JobSeekerDocument>?> GetByUserIdAsync(Guid userId);
}
