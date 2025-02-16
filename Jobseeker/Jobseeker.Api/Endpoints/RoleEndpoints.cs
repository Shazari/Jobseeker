using Jobseeker.Application.DTOs.Role;
using Jobseeker.Application.Services.Interfaces;

namespace Jobseeker.Api.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var roles = app.MapGroup("/roles").RequireAuthorization("AdminPolicy");

        // Get all roles
        roles.MapGet("/", async (IRoleService roleService) =>
        {
            var allRoles = await roleService.GetAllAsync();
            return Results.Ok(allRoles);
        });

        // Add new role
        roles.MapPost("/add", async (CreateRoleRequest request, IRoleService roleService) =>
        {
            var createdRole = await roleService.CreateAsync(request);
            return createdRole is not null
                ? Results.Created($"/roles/{createdRole.Id}", createdRole)
                : Results.BadRequest("Failed to add role.");
        });

        // Update role
        roles.MapPut("/update", async (UpdateRoleRequest request, IRoleService roleService) =>
        {
            var updatedRole = await roleService.UpdateAsync(request);
            return updatedRole is not null
                ? Results.Ok(updatedRole)
                : Results.NotFound("Role not found.");
        });

        // Delete role
        roles.MapDelete("/{id:guid}", async (Guid id, IRoleService roleService) =>
        {
            await roleService.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
