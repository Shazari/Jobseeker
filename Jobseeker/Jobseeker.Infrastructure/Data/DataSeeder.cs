using Jobseeker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Jobseeker.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure the database is created
        await context.Database.MigrateAsync();

        // Seed roles dynamically
        var defaultRoles = new List<Role>
        {
            new Role { Id = Guid.NewGuid(), Name = "Admin" },
            new Role { Id = Guid.NewGuid(), Name = "JobSeeker" },
            new Role { Id = Guid.NewGuid(), Name = "Company" }
        };

        foreach (var role in defaultRoles)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == role.Name))
            {
                await context.Roles.AddAsync(role);
            }
        }

        await context.SaveChangesAsync();

        // Seed an admin user with all roles
        const string adminEmail = "admin@jobseeker.com";
        const string adminPassword = "Admin@12345";

        var adminUser = await context.Users.Include(u => u.Roles)
                                           .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "System Administrator",
                Email = adminEmail,
                PhoneNumber = "1234567890",
                Type = Domain.Enums.UserType.JobSeeker,
                CreatedAt = DateTime.UtcNow,
                Roles = await context.Roles.ToListAsync()
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Create the admin user in Firebase with all roles
            try
            {
                var userRecordArgs = new UserRecordArgs
                {
                    Email = adminEmail,
                    Password = adminPassword,
                    DisplayName = adminUser.FullName
                };

                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                var roles = adminUser.Roles.Select(r => r.Name).ToList();
                var claims = new Dictionary<string, object>
                {
                    { "roles", roles }
                };

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);

                Console.WriteLine("Admin user created successfully with all roles.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create admin user in Firebase: {ex.Message}");
            }
        }
        else
        {
            // Ensure the admin user has all roles
            var allRoles = await context.Roles.ToListAsync();
            var missingRoles = allRoles.Where(role => !adminUser.Roles.Any(r => r.Name == role.Name)).ToList();

            if (missingRoles.Any())
            {
                adminUser.Roles.AddRange(missingRoles);
                await context.SaveChangesAsync();

                try
                {
                    var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(adminEmail);
                    var updatedRoles = adminUser.Roles.Select(r => r.Name).ToList();
                    var claims = new Dictionary<string, object>
                    {
                        { "roles", updatedRoles }
                    };

                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, claims);

                    Console.WriteLine("Admin user roles updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update admin user roles in Firebase: {ex.Message}");
                }
            }
        }
    }
}
