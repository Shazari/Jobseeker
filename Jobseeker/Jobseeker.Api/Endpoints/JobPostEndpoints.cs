using Jobseeker.Application.DTOs.JobPost;
using Jobseeker.Application.Services.Interfaces;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class JobPostEndpoints
{
    public static void MapJobPostEndpoints(this IEndpointRouteBuilder app)
    {
        var jobs = app.MapGroup("/jobs");

        // ➤ Get All Job Posts
        jobs.MapGet("/", async (IJobPostService jobService) =>
        {
            Log.Information("Fetching all job posts");

            var jobPosts = await jobService.GetAllAsync();
            if (jobPosts.Any())
            {
                Log.Information("Retrieved {Count} job posts", jobPosts.Count);
                return Results.Ok(jobPosts);
            }
            else
            {
                Log.Warning("No job posts found");
                return Results.NotFound("No job posts found.");
            }
        });

        // ➤ Get Job Post By ID
        jobs.MapGet("/{id:guid}", async (Guid id, IJobPostService jobService) =>
        {
            Log.Information("Fetching job post with ID: {Id}", id);

            var job = await jobService.GetByIdAsync(id);
            if (job is not null)
            {
                Log.Information("Found job post with ID: {Id}", id);
                return Results.Ok(job);
            }
            else
            {
                Log.Warning("No job post found with ID: {Id}", id);
                return Results.NotFound("Job not found.");
            }
        });

        // ➤ Create Job Post
        jobs.MapPost("/", async (CreateJobPostRequest createDto, IJobPostService jobService) =>
        {
            Log.Information("Creating a new job post with Title: {Title}, Company: {Company}", createDto.Title, createDto.CompanyName);

            var newJob = await jobService.CreateAsync(createDto);
            if (newJob != null)
            {
                Log.Information("Successfully created job post with ID: {Id}", newJob.Id);
                return Results.Created($"/jobs/{newJob.Id}", newJob);
            }
            else
            {
                Log.Warning("Failed to create job post with Title: {Title}", createDto.Title);
                return Results.BadRequest("Failed to create job post.");
            }
        });

        // ➤ Update Job Post
        jobs.MapPut("/{id:guid}", async (Guid id, UpdateJobPostRequest updateDto, IJobPostService jobService) =>
        {
            Log.Information("Updating job post with ID: {Id}", id);

            if (id != updateDto.Id)
            {
                Log.Warning("Mismatched IDs: Route ID: {RouteId}, DTO ID: {DtoId}", id, updateDto.Id);
                return Results.BadRequest("Mismatched IDs.");
            }

            var updatedJob = await jobService.UpdateAsync(updateDto);
            if (updatedJob != null)
            {
                Log.Information("Successfully updated job post with ID: {Id}", id);
                return Results.Ok(updatedJob);
            }
            else
            {
                Log.Warning("Job post with ID: {Id} not found", id);
                return Results.NotFound();
            }
        });

        // ➤ Delete Job Post
        jobs.MapDelete("/{id:guid}", async (Guid id, IJobPostService jobService) =>
        {
            Log.Information("Deleting job post with ID: {Id}", id);

            await jobService.DeleteAsync(id);
            Log.Information("Job post with ID: {Id} deleted successfully", id);
            return Results.NoContent();
        });
    }
}
