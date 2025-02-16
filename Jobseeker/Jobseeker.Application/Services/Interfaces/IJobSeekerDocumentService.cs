using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;

namespace Jobseeker.Application.Services.Interfaces;

public interface IJobSeekerDocumentService : IBaseService<JobSeekerDocumentDto, CreateJobSeekerDocumentRequest, UpdateJobSeekerDocumentRequest>
{
    Task<IList<JobSeekerDocumentDto>> GetByJobSeekerIdAsync(Guid jobSeekerId);
    Task<IList<JobSeekerDocumentDto>> GetByJobSeekerEmailAsync(string email);
    Task<JobSeekerDocumentDto> UploadDocumentAsync(FileUpload file, string type, string email);
    Task<bool> DeleteDocumentAsync(Guid documentId, string email);
}