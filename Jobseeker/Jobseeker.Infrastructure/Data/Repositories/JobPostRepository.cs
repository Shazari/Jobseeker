using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class JobPostRepository : Repository<JobPost>, IJobPostRepository
{
    internal JobPostRepository(ApplicationDbContext databaseContext) : base(databaseContext)
    {
    }

    public async Task<IList<JobPost>?> GetByCompanyAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return null;
        }

        var result =
            await DbSet
            .Where(jp => jp.CompanyName != null
                                && jp.CompanyName.ToLower() == companyName.ToLower())
            .ToListAsync();

        return result;
    }
}
