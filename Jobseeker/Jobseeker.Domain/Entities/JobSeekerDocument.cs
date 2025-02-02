using Jobseeker.Domain.Common;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Domain.Entities;

public class JobSeekerDocument : BaseEntity
{
    public Guid JobSeekerId { get; set; }
    public required string DocumentUrl { get; set; }
    public DocumentType Type { get; set; }
}
