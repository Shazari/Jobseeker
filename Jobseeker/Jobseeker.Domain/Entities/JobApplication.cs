using Jobseeker.Domain.Common;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Domain.Entities;

public class JobApplication : AuditableEntity
{
    public Guid JobSeekerId { get; set; }
    public Guid JobId { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    public List<JobSeekerDocument> Documents { get; set; } = [];
}
