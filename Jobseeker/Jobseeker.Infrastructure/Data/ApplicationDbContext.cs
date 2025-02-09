using Jobseeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<JobPost> JobPosts => Set<JobPost>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobSeekerDocument> Documents => Set<JobSeekerDocument>();

}
