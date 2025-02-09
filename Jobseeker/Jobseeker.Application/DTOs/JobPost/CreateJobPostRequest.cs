using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobPost;

public record CreateJobPostRequest(
    string Title,
    string? Description,
    string? CompanyName,
    string? Location,
    JobType Type,
    Guid? EmployerId
);