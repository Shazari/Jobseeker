using Jobseeker.Domain.Common;
using Jobseeker.Domain.Enums;

namespace Jobseeker.Domain.Entities;

public class User : AuditableEntity
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public UserType Type { get; set; }
    public List<JobApplication> JobApplications { get; set; } = [];
    public List<JobSeekerDocument> Documents { get; set; } = [];
    public List<Role> Roles { get; set; } = [];
}
