using Jobseeker.Application.DTOs.JobApplication;
using Jobseeker.Application.Services.Interfaces;

namespace Jobseeker.Api.Endpoints;

public static class JobApplicationEndpoints
{
    public static void MapJobApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var applications = app.MapGroup("/applications");

        applications.MapGet("/", async (IJobApplicationService jobAppService) =>
        {
            var applications = await jobAppService.GetAllAsync();
            return Results.Ok(applications);
        });

        applications.MapGet("/{id:guid}", async (Guid id, IJobApplicationService jobAppService) =>
        {
            var application = await jobAppService.GetByIdAsync(id);
            return application is not null ? Results.Ok(application) : Results.NotFound();
        });

        applications.MapPost("/", async (CreateJobApplicationRequest createDto, IJobApplicationService jobAppService) =>
        {
            var newApplication = await jobAppService.CreateAsync(createDto);
            return Results.Created($"/applications/{newApplication.Id}", newApplication);
        });

        applications.MapPut("/{id:guid}", async (Guid id, UpdateJobApplicationRequest updateDto, IJobApplicationService jobAppService) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs");
            var updatedApplication = await jobAppService.UpdateAsync(updateDto);
            return updatedApplication is not null ? Results.Ok(updatedApplication) : Results.NotFound();
        });

        applications.MapDelete("/{id:guid}", async (Guid id, IJobApplicationService jobAppService) =>
        {
            await jobAppService.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
