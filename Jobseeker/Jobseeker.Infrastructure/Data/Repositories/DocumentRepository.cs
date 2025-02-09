using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class DocumentRepository : Repository<JobSeekerDocument>, IDocumentRepository
{
    internal DocumentRepository(ApplicationDbContext databaseContext) : base(databaseContext)
    {
    }

    public async Task<IList<JobSeekerDocument>?> GetByUserIdAsync(Guid userId)
    {
        if(userId == Guid.Empty)
        {
            return null;
        }

        var result =
            await DbSet
            .Where(doc => doc.JobSeekerId == userId)
            .ToListAsync();

        return result;
    }
}
