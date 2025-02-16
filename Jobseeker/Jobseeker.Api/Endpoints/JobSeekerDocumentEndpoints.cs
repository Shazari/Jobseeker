using System.Security.Claims;
using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Jobseeker.Api.Endpoints;

public static class JobSeekerDocumentEndpoints
{
    public static void MapJobSeekerDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var documents = app.MapGroup("/documents").RequireAuthorization();

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
        documents.MapPost("/upload", async (
         ClaimsPrincipal user,
         [FromForm] IFormFile file,
         string type,
         IJobSeekerDocumentService documentService) =>
        {
                var email = user.FindFirst(ClaimTypes.Email)?.Value
                            ?? user.FindFirst("email")?.Value;

                try
                {
                    // Map IFormFile to FileUpload abstraction.
                    var fileUpload = new FileUpload(
                        file.OpenReadStream(),
                        file.FileName,
                        file.ContentType,
                        file.Length);

                    var document = await documentService.UploadDocumentAsync(fileUpload, type, email);
                    return Results.Ok(new { FileUrl = document.DocumentUrl, DocumentId = document.Id });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
        }).DisableAntiforgery();

        // ➤ Update Document Info
        documents.MapPut("/{id:guid}", async (Guid id, UpdateJobSeekerDocumentRequest updateDto, IJobSeekerDocumentService service) =>
        {
            if (id != updateDto.Id) return Results.BadRequest("Mismatched IDs.");
            var updatedDoc = await service.UpdateAsync(updateDto);
            return updatedDoc is not null ? Results.Ok(updatedDoc) : Results.NotFound($"Document with ID {id} not found.");
        });

        // ➤ Delete Document
        documents.MapDelete("/delete", async (Guid documentId, ClaimsPrincipal user, IJobSeekerDocumentService documentService) =>
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value;

            var success = await documentService.DeleteDocumentAsync(documentId, email!);
            return success
                ? Results.Ok("Document and file deleted successfully.")
                : Results.BadRequest("Deletion failed. Either the document does not exist, it does not belong to you, or file deletion failed.");
        }).DisableAntiforgery();
    }
}