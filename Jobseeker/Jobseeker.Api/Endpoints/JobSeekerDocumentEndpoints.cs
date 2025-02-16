using System.Security.Claims;
using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Jobseeker.Api.Endpoints;

public static class JobSeekerDocumentEndpoints
{
    public static void MapJobSeekerDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var documents = app.MapGroup("/documents").RequireAuthorization();

        // ➤ Get All Documents
        documents.MapGet("/", async (IJobSeekerDocumentService service) =>
        {
            Log.Information("Fetching all documents");

            var docs = await service.GetAllAsync();
            if (docs.Any())
            {
                Log.Information("Retrieved {Count} documents", docs.Count);
                return Results.Ok(docs);
            }
            else
            {
                Log.Warning("No documents found");
                return Results.NotFound("No documents found.");
            }
        });

        // ➤ Get Document By ID
        documents.MapGet("/{id:guid}", async (Guid id, IJobSeekerDocumentService service) =>
        {
            Log.Information("Fetching document with ID: {Id}", id);

            var doc = await service.GetByIdAsync(id);
            if (doc is not null)
            {
                Log.Information("Document found with ID: {Id}", id);
                return Results.Ok(doc);
            }
            else
            {
                Log.Warning("Document with ID: {Id} not found", id);
                return Results.NotFound($"Document with ID {id} not found.");
            }
        });

        // ➤ Get Documents By Job Seeker ID
        documents.MapGet("/by-jobseeker/{jobSeekerId:guid}", async (Guid jobSeekerId, IJobSeekerDocumentService service) =>
        {
            Log.Information("Fetching documents for JobSeekerId: {JobSeekerId}", jobSeekerId);

            var docs = await service.GetByJobSeekerIdAsync(jobSeekerId);
            if (docs.Any())
            {
                Log.Information("Found {Count} documents for JobSeekerId: {JobSeekerId}", docs.Count, jobSeekerId);
                return Results.Ok(docs);
            }
            else
            {
                Log.Warning("No documents found for JobSeekerId: {JobSeekerId}", jobSeekerId);
                return Results.NotFound($"No documents found for job seeker ID {jobSeekerId}.");
            }
        });

        // ➤ Upload Document (with Firebase Storage)
        documents.MapPost("/upload", async (
         ClaimsPrincipal user,
         [FromForm] IFormFile file,
         string type,
         IJobSeekerDocumentService documentService) =>
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value
                        ?? user.FindFirst("email")?.Value;

            Log.Information("Uploading document for user: {Email} with type: {Type}", email, type);

            try
            {
                // Map IFormFile to FileUpload abstraction.
                var fileUpload = new FileUpload(
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType,
                    file.Length);

                var document = await documentService.UploadDocumentAsync(fileUpload, type, email);

                Log.Information("Document uploaded successfully. URL: {Url}, ID: {DocumentId}", document.DocumentUrl, document.Id);

                return Results.Ok(new { FileUrl = document.DocumentUrl, DocumentId = document.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to upload document for user: {Email}", email);
                return Results.BadRequest(ex.Message);
            }
        }).DisableAntiforgery();

        // ➤ Update Document Info
        documents.MapPut("/{id:guid}", async (Guid id, UpdateJobSeekerDocumentRequest updateDto, IJobSeekerDocumentService service) =>
        {
            Log.Information("Updating document with ID: {Id}", id);

            if (id != updateDto.Id)
            {
                Log.Warning("Mismatched IDs for document update. Route ID: {RouteId}, DTO ID: {DtoId}", id, updateDto.Id);
                return Results.BadRequest("Mismatched IDs.");
            }

            var updatedDoc = await service.UpdateAsync(updateDto);
            if (updatedDoc is not null)
            {
                Log.Information("Document with ID: {Id} updated successfully", id);
                return Results.Ok(updatedDoc);
            }
            else
            {
                Log.Warning("Document with ID: {Id} not found during update", id);
                return Results.NotFound($"Document with ID {id} not found.");
            }
        });

        // ➤ Delete Document
        documents.MapDelete("/delete", async (Guid documentId, ClaimsPrincipal user, IJobSeekerDocumentService documentService) =>
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value;

            Log.Information("Attempting to delete document with ID: {DocumentId} for user: {Email}", documentId, email);

            var success = await documentService.DeleteDocumentAsync(documentId, email!);
            if (success)
            {
                Log.Information("Document with ID: {DocumentId} deleted successfully for user: {Email}", documentId, email);
                return Results.Ok("Document and file deleted successfully.");
            }
            else
            {
                Log.Warning("Failed to delete document with ID: {DocumentId} for user: {Email}", documentId, email);
                return Results.BadRequest("Deletion failed. Either the document does not exist, it does not belong to you, or file deletion failed.");
            }
        }).DisableAntiforgery();
    }
}
