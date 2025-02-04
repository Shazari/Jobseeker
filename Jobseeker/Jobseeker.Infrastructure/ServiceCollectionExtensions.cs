using Jobseeker.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Jobseeker.Domain.Services;
using Microsoft.Extensions.Configuration;
using Jobseeker.Domain.Common;
using Jobseeker.Infrastructure.Data;
using Jobseeker.Infrastructure.Tool;
using Jobseeker.Infrastructure.Tool.Enum;
using System.Runtime.CompilerServices;
using System;

namespace Jobseeker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserAuthService, FirebaseAuthService>();
        services.AddScoped<IFileStorageService, FirebaseStorageService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>(sp =>
        {
            var configuration = sp.GetService<IConfiguration>();
            DbOptions options =
                new DbOptions
                {
                    Provider =
                    (Provider)
                        Convert.ToInt32(configuration?.GetSection(key: "databaseProvider").Value),

                    ConnectionString =
                        configuration?.GetSection(key: "ConnectionStrings").GetSection(key: "DbConnectionString").Value,
                };

            return new UnitOfWork(dbOptions: options);
        });

        return services;
    }
}
