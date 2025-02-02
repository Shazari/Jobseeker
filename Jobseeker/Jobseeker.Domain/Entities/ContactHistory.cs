using Jobseeker.Domain.Common;

namespace Jobseeker.Domain.Entities;

public class ContactHistory : BaseEntity
{
    public Guid JobSeekerId { get; set; }
    public Guid JobId { get; set; }
    public required string Notes { get; set; }
    public DateTime ContactedOn { get; set; } = DateTime.UtcNow;
}
