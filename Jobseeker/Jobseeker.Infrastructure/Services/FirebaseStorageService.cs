using Firebase.Storage;
using Jobseeker.Domain.Services;

namespace Jobseeker.Infrastructure.Services;

public class FirebaseStorageService : IFileStorageService
{
    private readonly FirebaseStorage _firebaseStorage = new("your-firebase-bucket.appspot.com");

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var fileReference = _firebaseStorage.Child("uploads").Child(fileName);
        return await fileReference.PutAsync(fileStream);
    }
}
