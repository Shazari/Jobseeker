using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobApplication;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.DTOs.User;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class UserService
    : BaseService<User, UserDto, CreateUserRequest, UpdateUserRequest>,
      IUserService
{
    public UserService(IRepository<User> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }

    protected override UserDto MapToDto(User entity)
        => new(entity.Id, entity.FullName, entity.Email, entity.PhoneNumber, entity.Type, entity.JobApplications.Select(a => new JobApplicationDto(a.Id, a.JobSeekerId, a.JobId, a.Status, a.AppliedDate, new())).ToList(), entity.Documents.Select(d => new JobSeekerDocumentDto(d.Id, d.JobSeekerId, d.DocumentUrl, d.Type)).ToList(), entity.CreatedAt, entity.UpdatedAt);

    protected override User MapToEntity(CreateUserRequest createDto)
        => new() { FullName = createDto.FullName, Email = createDto.Email, PhoneNumber = createDto.PhoneNumber, Type = createDto.Type };

    protected override void MapToExistingEntity(UpdateUserRequest updateDto, User entity)
    {
        entity.FullName = updateDto.FullName;
        entity.Email = updateDto.Email;
        entity.PhoneNumber = updateDto.PhoneNumber;
        entity.Type = updateDto.Type;
    }

    protected override Guid GetEntityIdFromUpdateDto(UpdateUserRequest updateDto)
        => updateDto.Id;
}
