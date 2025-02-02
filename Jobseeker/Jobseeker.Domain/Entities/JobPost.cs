using Jobseeker.Domain.Common;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Domain.Entities;

public class JobPost : AuditableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string CompanyName { get; set; }
    public string? Location { get; set; }
    public JobType Type { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public User? Employer { get; set; }
}
