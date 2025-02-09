using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.User;

namespace Jobseeker.Application.Services.Interfaces;

public interface IUserService : IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>
{
}
