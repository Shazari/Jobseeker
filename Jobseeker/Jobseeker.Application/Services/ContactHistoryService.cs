using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.ContactHistory;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class ContactHistoryService
    : BaseService<ContactHistory, ContactHistoryDto, CreateContactHistoryRequest, UpdateContactHistoryRequest>,
      IContactHistoryService
{
    public ContactHistoryService(IRepository<ContactHistory> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }

    protected override ContactHistoryDto MapToDto(ContactHistory entity)
        => new(entity.Id, entity.JobSeekerId, entity.JobId, entity.Notes, entity.ContactedOn);

    protected override ContactHistory MapToEntity(CreateContactHistoryRequest createDto)
        => new() { JobSeekerId = createDto.JobSeekerId, JobId = createDto.JobId, Notes = createDto.Notes };

    protected override void MapToExistingEntity(UpdateContactHistoryRequest updateDto, ContactHistory entity)
        => entity.Notes = updateDto.Notes;

    protected override Guid GetEntityIdFromUpdateDto(UpdateContactHistoryRequest updateDto)
        => updateDto.Id;
}
