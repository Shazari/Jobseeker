using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.Role;

namespace Jobseeker.Application.Services.Interfaces;

public interface IRoleService : IBaseService<RoleDto, CreateRoleRequest, UpdateRoleRequest>
{
}
