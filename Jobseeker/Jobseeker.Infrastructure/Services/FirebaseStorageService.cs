using Firebase.Storage;
using Jobseeker.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace Jobseeker.Infrastructure.Services;

public class FirebaseStorageService : IFileStorageService
{
    private readonly FirebaseStorage _firebaseStorage;

    public FirebaseStorageService(IConfiguration configuration)
    {
        var bucket = configuration["Firebase:Bucket"]
            ?? throw new InvalidOperationException("Firebase bucket name is missing in configuration.");

        _firebaseStorage = new FirebaseStorage(bucket);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var fileRef = _firebaseStorage
                .Child("uploads")
                .Child(fileName);

            var downloadUrl = await fileRef.PutAsync(fileStream);
            return downloadUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"File upload failed: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var segments = fileUrl.Split('/');
            var fileName = segments[^1];
            var fileRef = _firebaseStorage
                .Child("uploads")
                .Child(fileName);

            await fileRef.DeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete file: {ex.Message}");
            return false;
        }
    }
}
