using Jobseeker.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Jobseeker.Domain.Services;
using Microsoft.Extensions.Configuration;
using Jobseeker.Domain.Common;
using Jobseeker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Jobseeker.Infrastructure.Data.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }


    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Initialize Firebase using environment variable
        var firebaseCredentialPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
        if (string.IsNullOrWhiteSpace(firebaseCredentialPath))
        {
            throw new InvalidOperationException("Firebase credentials not found. Set the FIREBASE_CREDENTIALS environment variable.");
        }

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(firebaseCredentialPath)
            });
        }

        var firebaseProjectId = configuration["Firebase:ProjectId"];
        if (string.IsNullOrWhiteSpace(firebaseProjectId))
        {
            throw new InvalidOperationException("Firebase project ID is missing in configuration.");
        }

        // Configure JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
                    ValidateAudience = true,
                    ValidAudience = firebaseProjectId,
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var rolesClaim = context.Principal?.FindFirst("roles")?.Value;

                        if (!string.IsNullOrEmpty(rolesClaim))
                        {
                            var roles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rolesClaim);
                            var identity = context.Principal?.Identity as ClaimsIdentity;

                            if (identity != null && roles != null)
                            {
                                foreach (var role in roles)
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // Add Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("JobSeekerPolicy", policy => policy.RequireRole("JobSeeker"));
            options.AddPolicy("CompanyPolicy", policy => policy.RequireRole("Company"));
        });

        return services;
    }
}
