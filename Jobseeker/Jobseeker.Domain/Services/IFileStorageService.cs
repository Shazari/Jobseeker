namespace Jobseeker.Domain.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
}
