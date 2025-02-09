using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobApplication;

public record CreateJobApplicationRequest(
    Guid JobSeekerId,
    Guid JobId,
    JobApplicationStatus Status,
    List<CreateJobSeekerDocumentRequest> Documents
);
