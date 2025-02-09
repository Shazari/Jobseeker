using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobSeekerDocument;

public record UpdateJobSeekerDocumentRequest(
    Guid Id,
    string DocumentUrl,
    DocumentType Type
);