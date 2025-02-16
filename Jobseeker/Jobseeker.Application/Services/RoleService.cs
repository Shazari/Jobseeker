using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.Role;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;

namespace Jobseeker.Application.Services;

public class RoleService : BaseService<Role, RoleDto, CreateRoleRequest, UpdateRoleRequest>, IRoleService
{
    public RoleService(IRepository<Role> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }

    protected override RoleDto MapToDto(Role entity)
    {
        return new RoleDto(entity.Id, entity.Name);
    }

    protected override Role MapToEntity(CreateRoleRequest createDto)
    {
        return new Role
        {
            Name = createDto.Name
        };
    }

    protected override void MapToExistingEntity(UpdateRoleRequest updateDto, Role entity)
    {
        entity.Name = updateDto.Name;
    }

    protected override Guid GetEntityIdFromUpdateDto(UpdateRoleRequest updateDto)
    {
        return updateDto.Id;
    }
}