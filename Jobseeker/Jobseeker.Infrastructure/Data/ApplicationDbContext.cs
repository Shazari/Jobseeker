using Jobseeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) 
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobSeekerDocument> Documents => Set<JobSeekerDocument>();

}
