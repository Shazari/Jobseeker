using Jobseeker.Application.DTOs.Auth;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth");

        // Register User
        authGroup.MapPost("/register", async ([FromBody] RegisterUserRequest request, IUserAuthService authService) =>
        {
            Log.Information("Registering new user: {Email}, Roles: {Roles}", request.Email, string.Join(", ", request.Roles));

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Type = request.UserType
            };

            var success = await authService.RegisterUserAsync(user, request.Password, request.Roles);
            if (success)
            {
                Log.Information("User {Email} registered successfully", user.Email);
                return Results.Created($"/auth/register/{user.Email}", "User registered successfully");
            }
            else
            {
                Log.Warning("Failed to register user: {Email}", user.Email);
                return Results.BadRequest("Failed to register user");
            }
        });

        // Login User
        authGroup.MapPost("/login", async ([FromBody] LoginRequest request, IUserAuthService authService) =>
        {
            Log.Information("Login attempt for user: {Email}", request.Email);

            var token = await authService.AuthenticateUserAsync(request.Email, request.Password);
            if (token != null)
            {
                Log.Information("User {Email} logged in successfully", request.Email);
                return Results.Ok(new { Token = token });
            }
            else
            {
                Log.Warning("Unauthorized login attempt for user: {Email}", request.Email);
                return Results.Unauthorized();
            }
        });

        // Get User Roles
        authGroup.MapGet("/roles", async ([FromQuery] string email, IUserAuthService authService) =>
        {
            Log.Information("Fetching roles for user: {Email}", email);

            var roles = await authService.GetUserRolesAsync(email);
            if (roles.Any())
            {
                Log.Information("Roles retrieved for user: {Email}. Roles: {Roles}", email, string.Join(", ", roles));
                return Results.Ok(roles);
            }
            else
            {
                Log.Warning("No roles found for user: {Email}", email);
                return Results.NotFound("No roles found");
            }
        });

        // Update User Roles
        authGroup.MapPut("/roles/update", async ([FromBody] UpdateUserRolesRequest request, IUserAuthService authService) =>
        {
            Log.Information("Updating roles for user: {Email}. New Roles: {Roles}", request.Email, string.Join(", ", request.Roles));

            var success = await authService.UpdateUserRolesAsync(request.Email, request.Roles);
            if (success)
            {
                Log.Information("User roles updated successfully for user: {Email}", request.Email);
                return Results.Ok("User roles updated successfully");
            }
            else
            {
                Log.Warning("Failed to update roles for user: {Email}", request.Email);
                return Results.BadRequest("Failed to update roles");
            }
        });
    }
}