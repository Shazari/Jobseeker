using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobApplication;

public record UpdateJobApplicationRequest(
    Guid Id,
    JobApplicationStatus Status,
    List<UpdateJobSeekerDocumentRequest> Documents
);