using Jobseeker.Application.DTOs.JobApplication;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string? FullName,
    string? Email,
    string? PhoneNumber,
    UserType Type,
    List<JobApplicationDto> JobApplications,
    List<JobSeekerDocumentDto> Documents,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
