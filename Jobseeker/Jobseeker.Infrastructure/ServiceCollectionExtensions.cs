using Jobseeker.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Jobseeker.Domain.Services;
using Microsoft.Extensions.Configuration;
using Jobseeker.Domain.Common;
using Jobseeker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserAuthService, FirebaseAuthService>();
        services.AddScoped<IFileStorageService, FirebaseStorageService>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DbConnectionString"));
        });
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
