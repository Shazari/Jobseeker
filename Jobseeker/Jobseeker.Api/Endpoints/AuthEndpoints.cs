using Jobseeker.Application.DTOs.Auth;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jobseeker.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth");

        // Register User
        authGroup.MapPost("/register", async ([FromBody] RegisterUserRequest request, IUserAuthService authService) =>
        {
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Type = request.UserType
            };

            var success = await authService.RegisterUserAsync(user, request.Password, request.Roles);
            return success ? Results.Created($"/auth/register/{user.Email}", "User registered successfully") : Results.BadRequest("Failed to register user");
        });

        // Login User
        authGroup.MapPost("/login", async ([FromBody] LoginRequest request, IUserAuthService authService) =>
        {
            var token = await authService.AuthenticateUserAsync(request.Email, request.Password);
            return token != null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
        });

        // Get User Roles
        authGroup.MapGet("/roles", async ([FromQuery] string email, IUserAuthService authService) =>
        {
            var roles = await authService.GetUserRolesAsync(email);
            return roles.Any() ? Results.Ok(roles) : Results.NotFound("No roles found");
        });

        // Update User Roles
        authGroup.MapPut("/roles/update", async ([FromBody] UpdateUserRolesRequest request, IUserAuthService authService) =>
        {
            var success = await authService.UpdateUserRolesAsync(request.Email, request.Roles);
            return success ? Results.Ok("User roles updated successfully") : Results.BadRequest("Failed to update roles");
        });
    }
}