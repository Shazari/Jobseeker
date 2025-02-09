using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.ContactHistory;

namespace Jobseeker.Application.Services.Interfaces;

public interface IContactHistoryService : IBaseService<ContactHistoryDto, CreateContactHistoryRequest, UpdateContactHistoryRequest>
{
}
