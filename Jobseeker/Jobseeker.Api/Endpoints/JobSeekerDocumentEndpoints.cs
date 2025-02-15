using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Services;

namespace Jobseeker.Api.Endpoints;

public static class JobSeekerDocumentEndpoints
{
    public static void MapJobSeekerDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var documents = app.MapGroup("/documents");

        // ➤ Get All Documents
        documents.MapGet("/", async (IJobSeekerDocumentService service) =>
        {
            var docs = await service.GetAllAsync();
            return docs.Any() ? Results.Ok(docs) : Results.NotFound("No documents found.");
        });

        // ➤ Get Document By ID
        documents.MapGet("/{id:guid}", async (Guid id, IJobSeekerDocumentService service) =>
        {
            var doc = await service.GetByIdAsync(id);
            return doc is not null ? Results.Ok(doc) : Results.NotFound($"Document with ID {id} not found.");
        });

        // ➤ Get Documents By Job Seeker ID
        documents.MapGet("/by-jobseeker/{jobSeekerId:guid}", async (Guid jobSeekerId, IJobSeekerDocumentService service) =>
        {
            var docs = await service.GetByJobSeekerIdAsync(jobSeekerId);
            return docs.Any() ? Results.Ok(docs) : Results.NotFound($"No documents found for job seeker ID {jobSeekerId}.");
        });

        // ➤ Upload Document (with Firebase Storage)
        documents.MapPost("/upload", async (HttpRequest request, IFileStorageService fileService, IJobSeekerDocumentService docService) =>
        {
            var form = await request.ReadFormAsync();

            if (form.Files.Count == 0) return Results.BadRequest("No file provided.");

            var file = form.Files[0];
            if (!form.TryGetValue("jobSeekerId", out var jobSeekerIdValue) || !Guid.TryParse(jobSeekerIdValue, out var jobSeekerId))
            {
                return Results.BadRequest("Invalid or missing jobSeekerId.");
            }
            var documentType = form["documentType"];

            if (!Enum.TryParse(documentType, out Jobseeker.Domain.Enums.DocumentType type))
            {
                return Results.BadRequest("Invalid document type.");
            }

            // Upload file to Firebase Storage
            var fileName = $"{jobSeekerId}_{file.FileName}";
            using var stream = file.OpenReadStream();
            var fileUrl = await fileService.UploadFileAsync(stream, fileName);

            // Save document info in DB
            var createDto = new CreateJobSeekerDocumentRequest(jobSeekerId, fileUrl, type);
            var newDoc = await docService.CreateAsync(createDto);

            return Results.Created($"/documents/{newDoc.Id}", newDoc);
        });

        // ➤ Update Document Info
        documents.MapPut("/{id:guid}", async (Guid id, UpdateJobSeekerDocumentRequest updateDto, IJobSeekerDocumentService service) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs.");
            var updatedDoc = await service.UpdateAsync(updateDto);
            return updatedDoc is not null ? Results.Ok(updatedDoc) : Results.NotFound($"Document with ID {id} not found.");
        });

        // ➤ Delete Document
        documents.MapDelete("/{id:guid}", async (Guid id, IJobSeekerDocumentService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
