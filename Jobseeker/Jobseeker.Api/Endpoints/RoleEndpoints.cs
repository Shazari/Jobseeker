using Jobseeker.Application.DTOs.Role;
using Jobseeker.Application.Services.Interfaces;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var roles = app.MapGroup("/roles").RequireAuthorization("AdminPolicy");

        // ➤ Get all roles
        roles.MapGet("/", async (IRoleService roleService) =>
        {
            Log.Information("Fetching all roles");

            var allRoles = await roleService.GetAllAsync();
            if (allRoles.Any())
            {
                Log.Information("Retrieved {Count} roles", allRoles.Count);
                return Results.Ok(allRoles);
            }
            else
            {
                Log.Warning("No roles found in the system");
                return Results.NotFound("No roles found.");
            }
        });

        // ➤ Add new role
        roles.MapPost("/add", async (CreateRoleRequest request, IRoleService roleService) =>
        {
            Log.Information("Creating a new role with name: {RoleName}", request.Name);

            var createdRole = await roleService.CreateAsync(request);
            if (createdRole is not null)
            {
                Log.Information("Role '{RoleName}' created successfully with ID: {RoleId}", createdRole.Name, createdRole.Id);
                return Results.Created($"/roles/{createdRole.Id}", createdRole);
            }
            else
            {
                Log.Warning("Failed to create role with name: {RoleName}", request.Name);
                return Results.BadRequest("Failed to add role.");
            }
        });

        // ➤ Update role
        roles.MapPut("/update", async (UpdateRoleRequest request, IRoleService roleService) =>
        {
            Log.Information("Updating role with ID: {RoleId}", request.Id);

            var updatedRole = await roleService.UpdateAsync(request);
            if (updatedRole is not null)
            {
                Log.Information("Role with ID: {RoleId} updated successfully", request.Id);
                return Results.Ok(updatedRole);
            }
            else
            {
                Log.Warning("Failed to update role with ID: {RoleId}. Role not found", request.Id);
                return Results.NotFound("Role not found.");
            }
        });

        // ➤ Delete role
        roles.MapDelete("/{id:guid}", async (Guid id, IRoleService roleService) =>
        {
            Log.Information("Attempting to delete role with ID: {RoleId}", id);

            await roleService.DeleteAsync(id);

            Log.Information("Role with ID: {RoleId} deleted successfully", id);
            return Results.NoContent();
        });
    }
}
