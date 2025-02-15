using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class JobSeekerDocumentService
    : BaseService<JobSeekerDocument, JobSeekerDocumentDto, CreateJobSeekerDocumentRequest, UpdateJobSeekerDocumentRequest>,
      IJobSeekerDocumentService
{
    private readonly IRepository<JobSeekerDocument> repository;

    public JobSeekerDocumentService(IRepository<JobSeekerDocument> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) 
    {
        this.repository = repository;
    }

    protected override JobSeekerDocumentDto MapToDto(JobSeekerDocument entity)
        => new(entity.Id, entity.JobSeekerId, entity.DocumentUrl, entity.Type);

    protected override JobSeekerDocument MapToEntity(CreateJobSeekerDocumentRequest createDto)
        => new()
        {
            JobSeekerId = createDto.JobSeekerId,
            DocumentUrl = createDto.DocumentUrl,
            Type = createDto.Type
        };

    protected override void MapToExistingEntity(UpdateJobSeekerDocumentRequest updateDto, JobSeekerDocument entity)
    {
        entity.DocumentUrl = updateDto.DocumentUrl;
        entity.Type = updateDto.Type;
    }

    protected override Guid GetEntityIdFromUpdateDto(UpdateJobSeekerDocumentRequest updateDto)
        => updateDto.Id;

    public async Task<IList<JobSeekerDocumentDto>> GetByJobSeekerIdAsync(Guid jobSeekerId)
    {
        var documents = await repository.GetAllAsync();
        var filtered = documents.Where(d => d.JobSeekerId == jobSeekerId).ToList();
        return filtered.Select(MapToDto).ToList();
    }
}