using Jobseeker.Infrastructure.Tool.Enum;

namespace Jobseeker.Infrastructure.Tool;

public class DbOptions
{
    public Provider Provider { get; set; }
    
    public string? ConnectionString { get; set; }
}
