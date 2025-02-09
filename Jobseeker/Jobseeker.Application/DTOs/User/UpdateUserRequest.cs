using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.User;

public record UpdateUserRequest(
    Guid Id,
    string? FullName,
    string? Email,
    string? PhoneNumber,
    UserType Type
);