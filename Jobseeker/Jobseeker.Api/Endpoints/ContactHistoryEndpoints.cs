using Jobseeker.Application.DTOs.ContactHistory;
using Jobseeker.Application.Services.Interfaces;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class ContactHistoryEndpoints
{
    public static void MapContactHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var contacts = app.MapGroup("/contact-histories");

        // ➤ Get All Contact Histories
        contacts.MapGet("/", async (IContactHistoryService service) =>
        {
            Log.Information("Fetching all contact histories");

            var histories = await service.GetAllAsync();
            if (histories.Any())
            {
                Log.Information("Retrieved {Count} contact histories", histories.Count);
                return Results.Ok(histories);
            }
            else
            {
                Log.Warning("No contact histories found");
                return Results.NotFound("No contact histories found.");
            }
        });

        // ➤ Get Contact History By ID
        contacts.MapGet("/{id:guid}", async (Guid id, IContactHistoryService service) =>
        {
            Log.Information("Fetching contact history by ID: {Id}", id);

            var history = await service.GetByIdAsync(id);
            if (history is not null)
            {
                Log.Information("Contact history found for ID: {Id}", id);
                return Results.Ok(history);
            }
            else
            {
                Log.Warning("No contact history found with ID: {Id}", id);
                return Results.NotFound($"Contact history with ID {id} not found.");
            }
        });

        // ➤ Get Contact Histories By Job Seeker ID
        contacts.MapGet("/by-jobseeker/{jobSeekerId:guid}", async (Guid jobSeekerId, IContactHistoryService service) =>
        {
            Log.Information("Fetching contact histories for JobSeekerId: {JobSeekerId}", jobSeekerId);

            var histories = await service.GetAllAsync();
            var filtered = histories.Where(h => h.JobSeekerId == jobSeekerId).ToList();

            if (filtered.Any())
            {
                Log.Information("Found {Count} contact histories for JobSeekerId: {JobSeekerId}", filtered.Count, jobSeekerId);
                return Results.Ok(filtered);
            }
            else
            {
                Log.Warning("No contact histories found for JobSeekerId: {JobSeekerId}", jobSeekerId);
                return Results.NotFound($"No contact histories found for job seeker ID {jobSeekerId}.");
            }
        });

        // ➤ Create Contact History
        contacts.MapPost("/", async (CreateContactHistoryRequest createDto, IContactHistoryService service) =>
        {
            Log.Information("Creating new contact history for JobSeekerId: {JobSeekerId} and JobId: {JobId}", createDto.JobSeekerId, createDto.JobId);

            var newHistory = await service.CreateAsync(createDto);
            if (newHistory != null)
            {
                Log.Information("Contact history created successfully with ID: {Id}", newHistory.Id);
                return Results.Created($"/contact-histories/{newHistory.Id}", newHistory);
            }
            else
            {
                Log.Warning("Failed to create contact history for JobSeekerId: {JobSeekerId}", createDto.JobSeekerId);
                return Results.BadRequest("Failed to create contact history.");
            }
        });

        // ➤ Update Contact History
        contacts.MapPut("/{id:guid}", async (Guid id, UpdateContactHistoryRequest updateDto, IContactHistoryService service) =>
        {
            Log.Information("Updating contact history with ID: {Id}", id);

            if (id != updateDto.Id)
            {
                Log.Warning("Mismatched IDs for update. Route ID: {RouteId}, DTO ID: {DtoId}", id, updateDto.Id);
                return Results.BadRequest("Mismatched IDs.");
            }

            var updatedHistory = await service.UpdateAsync(updateDto);
            if (updatedHistory != null)
            {
                Log.Information("Successfully updated contact history with ID: {Id}", id);
                return Results.Ok(updatedHistory);
            }
            else
            {
                Log.Warning("Contact history with ID: {Id} not found", id);
                return Results.NotFound($"Contact history with ID {id} not found.");
            }
        });

        // ➤ Delete Contact History
        contacts.MapDelete("/{id:guid}", async (Guid id, IContactHistoryService service) =>
        {
            Log.Information("Deleting contact history with ID: {Id}", id);

            await service.DeleteAsync(id);
            Log.Information("Contact history with ID: {Id} deleted successfully", id);
            return Results.NoContent();
        });
    }
}
