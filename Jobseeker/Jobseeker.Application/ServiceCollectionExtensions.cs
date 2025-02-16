using Jobseeker.Application.Services;
using Jobseeker.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Jobseeker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();
        services.AddScoped<IJobPostService, JobPostService>();
        services.AddScoped<IContactHistoryService, ContactHistoryService>();
        services.AddScoped<IJobSeekerDocumentService, JobSeekerDocumentService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
