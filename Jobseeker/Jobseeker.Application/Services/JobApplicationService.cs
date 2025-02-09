using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobApplication;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class JobApplicationService
    : BaseService<JobApplication, JobApplicationDto, CreateJobApplicationRequest, UpdateJobApplicationRequest>,
      IJobApplicationService
{
    public JobApplicationService(IRepository<JobApplication> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }

    protected override JobApplicationDto MapToDto(JobApplication entity)
        => new(entity.Id, entity.JobSeekerId, entity.JobId, entity.Status, entity.AppliedDate, entity.Documents.Select(d => new JobSeekerDocumentDto(d.Id, d.JobSeekerId, d.DocumentUrl, d.Type)).ToList());

    protected override JobApplication MapToEntity(CreateJobApplicationRequest createDto)
        => new() { JobSeekerId = createDto.JobSeekerId, JobId = createDto.JobId, Status = createDto.Status, AppliedDate = DateTime.UtcNow };

    protected override void MapToExistingEntity(UpdateJobApplicationRequest updateDto, JobApplication entity)
        => entity.Status = updateDto.Status;

    protected override Guid GetEntityIdFromUpdateDto(UpdateJobApplicationRequest updateDto)
        => updateDto.Id;
}
