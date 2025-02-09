using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobPost;
using Jobseeker.Application.DTOs.User;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class JobPostService
    : BaseService<JobPost, JobPostDto, CreateJobPostRequest, UpdateJobPostRequest>,
      IJobPostService
{
    public JobPostService(IRepository<JobPost> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }

    protected override JobPostDto MapToDto(JobPost entity)
        => new(entity.Id, entity.Title, entity.Description, entity.CompanyName, entity.Location, entity.Type, entity.PostedDate, entity.Employer is null ? null : new UserDto(entity.Employer.Id, entity.Employer.FullName, entity.Employer.Email, entity.Employer.PhoneNumber, entity.Employer.Type, new(), new(), entity.Employer.CreatedAt, entity.Employer.UpdatedAt));

    protected override JobPost MapToEntity(CreateJobPostRequest createDto)
        => new() { Title = createDto.Title, Description = createDto.Description, CompanyName = createDto.CompanyName, Location = createDto.Location, Type = createDto.Type, PostedDate = DateTime.UtcNow };

    protected override void MapToExistingEntity(UpdateJobPostRequest updateDto, JobPost entity)
    {
        entity.Title = updateDto.Title;
        entity.Description = updateDto.Description;
        entity.CompanyName = updateDto.CompanyName;
        entity.Location = updateDto.Location;
        entity.Type = updateDto.Type;
    }

    protected override Guid GetEntityIdFromUpdateDto(UpdateJobPostRequest updateDto)
        => updateDto.Id;
}
