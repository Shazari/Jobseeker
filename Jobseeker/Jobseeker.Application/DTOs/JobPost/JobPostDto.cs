using Jobseeker.Application.DTOs.User;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.JobPost;

public record JobPostDto(
    Guid Id,
    string Title,
    string? Description,
    string? CompanyName,
    string? Location,
    JobType Type,
    DateTime PostedDate,
    UserDto? Employer
);
