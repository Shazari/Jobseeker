using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobApplication;

public record JobApplicationDto(
    Guid Id,
    Guid JobSeekerId,
    Guid JobId,
    JobApplicationStatus Status,
    DateTime AppliedDate,
    List<JobSeekerDocumentDto> Documents
);
