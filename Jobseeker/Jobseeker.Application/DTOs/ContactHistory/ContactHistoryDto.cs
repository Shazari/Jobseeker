namespace Jobseeker.Application.DTOs.ContactHistory;

public record ContactHistoryDto(
    Guid Id,
    Guid JobSeekerId,
    Guid JobId,
    string Notes,
    DateTime ContactedOn
);