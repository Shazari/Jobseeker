namespace Jobseeker.Application.DTOs.ContactHistory;

public record UpdateContactHistoryRequest(
    Guid Id,
    string Notes
);
