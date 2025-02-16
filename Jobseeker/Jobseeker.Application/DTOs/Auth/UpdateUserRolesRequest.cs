namespace Jobseeker.Application.DTOs.Auth;

public record UpdateUserRolesRequest(string Email, List<string> Roles);
