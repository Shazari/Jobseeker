using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobPost;

public record UpdateJobPostRequest(
    Guid Id,
    string Title,
    string? Description,
    string? CompanyName,
    string? Location,
    JobType Type,
    Guid? EmployerId
);
