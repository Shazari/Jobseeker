using Jobseeker.Application.DTOs.User;
using Jobseeker.Application.Services.Interfaces;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users");

        // ➤ Get User By ID
        users.MapGet("/{id:guid}", async (Guid id, IUserService userService) =>
        {
            Log.Information("Fetching user with ID: {UserId}", id);

            var user = await userService.GetByIdAsync(id);
            if (user is not null)
            {
                Log.Information("User with ID: {UserId} retrieved successfully", id);
                return Results.Ok(user);
            }
            else
            {
                Log.Warning("User with ID: {UserId} not found", id);
                return Results.NotFound("User not found.");
            }
        });

        // ➤ Update User
        users.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest updateDto, IUserService userService) =>
        {
            Log.Information("Attempting to update user with ID: {UserId}", id);

            if (id != updateDto.Id)
            {
                Log.Warning("Mismatched IDs: Route ID: {RouteId}, DTO ID: {DtoId}", id, updateDto.Id);
                return Results.BadRequest("Mismatched IDs");
            }

            var updatedUser = await userService.UpdateAsync(updateDto);
            if (updatedUser is not null)
            {
                Log.Information("User with ID: {UserId} updated successfully", id);
                return Results.Ok(updatedUser);
            }
            else
            {
                Log.Warning("Failed to update user with ID: {UserId}. User not found", id);
                return Results.NotFound();
            }
        });

        // ➤ Delete User
        users.MapDelete("/{id:guid}", async (Guid id, IUserService userService) =>
        {
            Log.Information("Attempting to delete user with ID: {UserId}", id);

            await userService.DeleteAsync(id);

            Log.Information("User with ID: {UserId} deleted successfully", id);
            return Results.NoContent();
        });
    }
}
