namespace Jobseeker.Application.DTOs.Auth;

public record RegisterUserRequest(string FullName, string Email, string PhoneNumber, string Password, Domain.Enums.UserType UserType, List<string> Roles);

