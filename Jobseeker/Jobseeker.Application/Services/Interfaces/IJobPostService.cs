using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobPost;

namespace Jobseeker.Application.Services.Interfaces;

public interface IJobPostService : IBaseService<JobPostDto, CreateJobPostRequest, UpdateJobPostRequest>
{
}
