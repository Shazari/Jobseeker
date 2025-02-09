using Jobseeker.Domain.Enums;

namespace Jobseeker.Application.DTOs.User;

public record CreateUserRequest(
    string? FullName,
    string? Email,
    string? PhoneNumber,
    UserType Type
);
