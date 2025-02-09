using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobSeekerDocument;

public record JobSeekerDocumentDto(
    Guid Id,
    Guid JobSeekerId,
    string DocumentUrl,
    DocumentType Type
);