namespace Jobseeker.Application.DTOs.ContactHistory;

public record CreateContactHistoryRequest(
    Guid JobSeekerId,
    Guid JobId,
    string Notes
);
