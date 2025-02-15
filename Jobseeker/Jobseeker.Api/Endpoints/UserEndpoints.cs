using Jobseeker.Application.DTOs.User;
using Jobseeker.Application.Services.Interfaces;

namespace Jobseeker.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users");

        //users.MapPost("/register", async (UserRegistrationDto userDto, IAuthService authService) =>
        //{
        //    var result = await authService.RegisterUserAsync(userDto.Email, userDto.Password);
        //    return result is not null ? Results.Ok(result) : Results.BadRequest("Registration failed.");
        //});

        //users.MapPost("/login", async (UserLoginDto userDto, IAuthService authService) =>
        //{
        //    var token = await authService.LoginUserAsync(userDto.Email, userDto.Password);
        //    return token is not null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
        //});

        users.MapGet("/{id:guid}", async (Guid id, IUserService userService) =>
        {
            var user = await userService.GetByIdAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound("User not found.");
        });

        users.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest updateDto, IUserService userService) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs");
            var updatedUser = await userService.UpdateAsync(updateDto);
            return updatedUser is not null ? Results.Ok(updatedUser) : Results.NotFound();
        });

        users.MapDelete("/{id:guid}", async (Guid id, IUserService userService) =>
        {
            await userService.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
