using Jobseeker.Application.DTOs.ContactHistory;
using Jobseeker.Application.Services.Interfaces;

namespace Jobseeker.Api.Endpoints;

public static class ContactHistoryEndpoints
{
    public static void MapContactHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var contacts = app.MapGroup("/contact-histories");

        // ➤ Get All Contact Histories
        contacts.MapGet("/", async (IContactHistoryService service) =>
        {
            var histories = await service.GetAllAsync();
            return histories.Any() ? Results.Ok(histories) : Results.NotFound("No contact histories found.");
        });

        // ➤ Get Contact History By ID
        contacts.MapGet("/{id:guid}", async (Guid id, IContactHistoryService service) =>
        {
            var history = await service.GetByIdAsync(id);
            return history is not null ? Results.Ok(history) : Results.NotFound($"Contact history with ID {id} not found.");
        });

        // ➤ Get Contact Histories By Job Seeker ID
        contacts.MapGet("/by-jobseeker/{jobSeekerId:guid}", async (Guid jobSeekerId, IContactHistoryService service) =>
        {
            var histories = await service.GetAllAsync();
            var filtered = histories.Where(h => h.JobSeekerId == jobSeekerId).ToList();
            return filtered.Any() ? Results.Ok(filtered) : Results.NotFound($"No contact histories found for job seeker ID {jobSeekerId}.");
        });

        // ➤ Create Contact History
        contacts.MapPost("/", async (CreateContactHistoryRequest createDto, IContactHistoryService service) =>
        {
            var newHistory = await service.CreateAsync(createDto);
            return Results.Created($"/contact-histories/{newHistory.Id}", newHistory);
        });

        // ➤ Update Contact History
        contacts.MapPut("/{id:guid}", async (Guid id, UpdateContactHistoryRequest updateDto, IContactHistoryService service) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs.");
            var updatedHistory = await service.UpdateAsync(updateDto);
            return updatedHistory is not null ? Results.Ok(updatedHistory) : Results.NotFound($"Contact history with ID {id} not found.");
        });

        // ➤ Delete Contact History
        contacts.MapDelete("/{id:guid}", async (Guid id, IContactHistoryService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
