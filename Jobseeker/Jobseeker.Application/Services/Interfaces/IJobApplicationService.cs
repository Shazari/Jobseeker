using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobApplication;

namespace Jobseeker.Application.Services.Interfaces;

public interface IJobApplicationService : IBaseService<JobApplicationDto, CreateJobApplicationRequest, UpdateJobApplicationRequest>
{
}
