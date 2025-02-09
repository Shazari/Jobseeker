using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;

namespace Jobseeker.Application.Services.Interfaces;

public interface IJobSeekerDocumentService : IBaseService<JobSeekerDocumentDto, CreateJobSeekerDocumentRequest, UpdateJobSeekerDocumentRequest>
{
}