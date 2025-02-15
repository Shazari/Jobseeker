using Jobseeker.Application.DTOs.JobPost;
using Jobseeker.Application.Services.Interfaces;

namespace Jobseeker.Api.Endpoints;

public static class JobPostEndpoints
{
    public static void MapJobPostEndpoints(this IEndpointRouteBuilder app)
    {
        var jobs = app.MapGroup("/jobs");

        jobs.MapGet("/", async (IJobPostService jobService) =>
        {
            var jobPosts = await jobService.GetAllAsync();
            return Results.Ok(jobPosts);
        });

        jobs.MapGet("/{id:guid}", async (Guid id, IJobPostService jobService) =>
        {
            var job = await jobService.GetByIdAsync(id);
            return job is not null ? Results.Ok(job) : Results.NotFound("Job not found.");
        });

        jobs.MapPost("/", async (CreateJobPostRequest createDto, IJobPostService jobService) =>
        {
            var newJob = await jobService.CreateAsync(createDto);
            return Results.Created($"/jobs/{newJob.Id}", newJob);
        });

        jobs.MapPut("/{id:guid}", async (Guid id, UpdateJobPostRequest updateDto, IJobPostService jobService) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs");
            var updatedJob = await jobService.UpdateAsync(updateDto);
            return updatedJob is not null ? Results.Ok(updatedJob) : Results.NotFound();
        });

        jobs.MapDelete("/{id:guid}", async (Guid id, IJobPostService jobService) =>
        {
            await jobService.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
