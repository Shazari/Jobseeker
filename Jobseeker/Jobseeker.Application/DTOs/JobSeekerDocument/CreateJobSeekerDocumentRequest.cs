using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobSeekerDocument;

public record CreateJobSeekerDocumentRequest(
    Guid JobSeekerId,
    string DocumentUrl,
    DocumentType Type
);