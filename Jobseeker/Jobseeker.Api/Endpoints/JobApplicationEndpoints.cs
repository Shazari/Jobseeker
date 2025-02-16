using Jobseeker.Application.DTOs.JobApplication;
using Jobseeker.Application.Services.Interfaces;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class JobApplicationEndpoints
{
    public static void MapJobApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var applications = app.MapGroup("/applications");

        // ➤ Get All Job Applications
        applications.MapGet("/", async (IJobApplicationService jobAppService) =>
        {
            Log.Information("Fetching all job applications");

            var apps = await jobAppService.GetAllAsync();
            if (apps.Any())
            {
                Log.Information("Retrieved {Count} job applications", apps.Count);
                return Results.Ok(apps);
            }
            else
            {
                Log.Warning("No job applications found");
                return Results.NotFound("No job applications found.");
            }
        });

        // ➤ Get Job Application By ID
        applications.MapGet("/{id:guid}", async (Guid id, IJobApplicationService jobAppService) =>
        {
            Log.Information("Fetching job application with ID: {Id}", id);

            var application = await jobAppService.GetByIdAsync(id);
            if (application is not null)
            {
                Log.Information("Found job application with ID: {Id}", id);
                return Results.Ok(application);
            }
            else
            {
                Log.Warning("No job application found with ID: {Id}", id);
                return Results.NotFound($"Job application with ID {id} not found.");
            }
        });

        // ➤ Create Job Application
        applications.MapPost("/", async (CreateJobApplicationRequest createDto, IJobApplicationService jobAppService) =>
        {
            Log.Information("Creating a new job application for JobSeekerId: {JobSeekerId}, JobId: {JobId}", createDto.JobSeekerId, createDto.JobId);

            var newApplication = await jobAppService.CreateAsync(createDto);
            if (newApplication != null)
            {
                Log.Information("Successfully created job application with ID: {Id}", newApplication.Id);
                return Results.Created($"/applications/{newApplication.Id}", newApplication);
            }
            else
            {
                Log.Warning("Failed to create job application for JobSeekerId: {JobSeekerId}", createDto.JobSeekerId);
                return Results.BadRequest("Failed to create job application.");
            }
        });

        // ➤ Update Job Application
        applications.MapPut("/{id:guid}", async (Guid id, UpdateJobApplicationRequest updateDto, IJobApplicationService jobAppService) =>
        {
            Log.Information("Updating job application with ID: {Id}", id);

            if (id != updateDto.Id)
            {
                Log.Warning("Mismatched IDs: Route ID: {RouteId}, DTO ID: {DtoId}", id, updateDto.Id);
                return Results.BadRequest("Mismatched IDs.");
            }

            var updatedApplication = await jobAppService.UpdateAsync(updateDto);
            if (updatedApplication != null)
            {
                Log.Information("Successfully updated job application with ID: {Id}", id);
                return Results.Ok(updatedApplication);
            }
            else
            {
                Log.Warning("Job application with ID: {Id} not found", id);
                return Results.NotFound($"Job application with ID {id} not found.");
            }
        });

        // ➤ Delete Job Application
        applications.MapDelete("/{id:guid}", async (Guid id, IJobApplicationService jobAppService) =>
        {
            Log.Information("Deleting job application with ID: {Id}", id);

            await jobAppService.DeleteAsync(id);
            Log.Information("Job application with ID: {Id} deleted successfully", id);
            return Results.NoContent();
        });
    }
}
