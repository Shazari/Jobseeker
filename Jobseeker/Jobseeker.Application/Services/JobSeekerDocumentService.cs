using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Enums;
using Jobseeker.Domain.Exceptions;
using Jobseeker.Domain.Services;
using Serilog;

namespace Jobseeker.Application.Services;

public class JobSeekerDocumentService
    : BaseService<JobSeekerDocument, JobSeekerDocumentDto, CreateJobSeekerDocumentRequest, UpdateJobSeekerDocumentRequest>,
      IJobSeekerDocumentService
{
    private readonly IRepository<JobSeekerDocument> repository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileStorageService fileStorageService;

    public JobSeekerDocumentService(IRepository<JobSeekerDocument> repository,
                                    IUnitOfWork unitOfWork,
                                    IFileStorageService fileStorageService)
        : base(repository, unitOfWork)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
        this.fileStorageService = fileStorageService;
    }

    protected override JobSeekerDocumentDto MapToDto(JobSeekerDocument entity)
        => new(entity.Id, entity.JobSeekerId, entity.DocumentUrl, entity.Type);

    protected override JobSeekerDocument MapToEntity(CreateJobSeekerDocumentRequest createDto)
        => new()
        {
            JobSeekerId = createDto.JobSeekerId,
            DocumentUrl = createDto.DocumentUrl,
            Type = createDto.Type
        };

    protected override void MapToExistingEntity(UpdateJobSeekerDocumentRequest updateDto, JobSeekerDocument entity)
    {
        entity.DocumentUrl = updateDto.DocumentUrl;
        entity.Type = updateDto.Type;
    }

    protected override Guid GetEntityIdFromUpdateDto(UpdateJobSeekerDocumentRequest updateDto)
        => updateDto.Id;

    public async Task<IList<JobSeekerDocumentDto>> GetByJobSeekerIdAsync(Guid jobSeekerId)
    {
        Log.Information("Fetching documents for JobSeeker ID: {JobSeekerId}", jobSeekerId);
        var documents = await repository.GetAllAsync();
        var filtered = documents.Where(d => d.JobSeekerId == jobSeekerId).ToList();
        Log.Information("Found {Count} documents for JobSeeker ID: {JobSeekerId}", filtered.Count, jobSeekerId);
        return filtered.Select(MapToDto).ToList();
    }

    public async Task<IList<JobSeekerDocumentDto>> GetByJobSeekerEmailAsync(string email)
    {
        Log.Information("Fetching documents for email: {Email}", email);
        var documents = await repository.GetAllAsync();
        var jobSeeker = await unitOfWork.UserRepository.GetByEmailAsync(email);
        if (jobSeeker == null)
        {
            Log.Warning("No job seeker found for email: {Email}", email);
            return new List<JobSeekerDocumentDto>();
        }
        var filtered = documents.Where(d => d.JobSeekerId == jobSeeker.Id).ToList();
        Log.Information("Found {Count} documents for email: {Email}", filtered.Count, email);
        return filtered.Select(MapToDto).ToList();
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId, string email)
    {
        Log.Information("Attempting to delete document with ID: {DocumentId} for user: {Email}", documentId, email);
        var document = await repository.GetByIdAsync(documentId);
        if (document == null)
        {
            Log.Warning("Document with ID: {DocumentId} not found", documentId);
            return false;
        }

        var jobSeeker = await unitOfWork.UserRepository.GetByEmailAsync(email);
        if (document.JobSeekerId != jobSeeker?.Id)
        {
            Log.Warning("Document with ID: {DocumentId} does not belong to user: {Email}", documentId, email);
            return false;
        }

        var fileDeleted = await fileStorageService.DeleteFileAsync(document.DocumentUrl);
        if (!fileDeleted)
        {
            Log.Error("Failed to delete file from storage: {FileUrl}", document.DocumentUrl);
            return false;
        }

        await repository.DeleteAsync(document);
        await unitOfWork.SaveAsync();
        Log.Information("Document with ID: {DocumentId} deleted successfully for user: {Email}", documentId, email);
        return true;
    }

    public async Task<JobSeekerDocumentDto> UploadDocumentAsync(FileUpload file, string type, string email)
    {
        Log.Information("Starting document upload for email: {Email}", email);

        if (!Enum.TryParse<DocumentType>(type, true, out var documentType))
        {
            Log.Warning("Invalid document type: {DocumentType}", type);
            throw new ArgumentException("Invalid document type.");
        }

        if (file == null || file.Length == 0)
        {
            Log.Warning("Attempted to upload an empty file for email: {Email}", email);
            throw new ArgumentException("No file uploaded.");
        }

        var userInDb = await unitOfWork.UserRepository.GetByEmailAsync(email);
        if (userInDb == null)
        {
            Log.Error("User not found for email: {Email}", email);
            throw new NotFoundException("User not found.");
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var downloadUrl = await fileStorageService.UploadFileAsync(file.Content, uniqueFileName, file.ContentType);

        Log.Information("File uploaded successfully. URL: {FileUrl}", downloadUrl);

        var createRequest = new CreateJobSeekerDocumentRequest(userInDb.Id, downloadUrl, documentType);
        var document = await CreateAsync(createRequest);

        Log.Information("Document record created successfully with ID: {DocumentId} for email: {Email}", document.Id, email);

        return document;
    }
}
