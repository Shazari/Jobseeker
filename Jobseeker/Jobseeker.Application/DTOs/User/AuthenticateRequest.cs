namespace Jobseeker.Application.DTOs.User;

public record AuthenticateRequest(
    string Email,
    string Password
);