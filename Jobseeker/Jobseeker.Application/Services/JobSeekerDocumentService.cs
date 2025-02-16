using Jobseeker.Application.Common;
using Jobseeker.Application.DTOs.JobSeekerDocument;
using Jobseeker.Application.Services.Interfaces;
using Jobseeker.Domain.Common;
using Jobseeker.Domain.Entities;
using Jobseeker.Domain.Enums;
using Jobseeker.Domain.Exceptions;
using Jobseeker.Domain.Services;

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
        var documents = await repository.GetAllAsync();
        var filtered = documents.Where(d => d.JobSeekerId == jobSeekerId).ToList();
        return filtered.Select(MapToDto).ToList();
    }

    public async Task<IList<JobSeekerDocumentDto>> GetByJobSeekerEmailAsync(string email)
    {
        var documents = await repository.GetAllAsync();
        var jobSeeker = await unitOfWork.UserRepository.GetByEmailAsync(email);
        var filtered = documents.Where(d => d.JobSeekerId == jobSeeker?.Id).ToList();
        return filtered.Select(MapToDto).ToList();
    }

    public async Task<bool> DeleteDocumentAsync(Guid documentId, string email)
    {
        var document = await repository.GetByIdAsync(documentId);
        if (document == null || string.IsNullOrEmpty(email))
        {
            return false;
        }

        var jobSeeker = await unitOfWork.UserRepository.GetByEmailAsync(email);

        if (document.JobSeekerId != jobSeeker?.Id)
        {
            return false;
        }

        var fileDeleted = await fileStorageService.DeleteFileAsync(document.DocumentUrl);
        if (!fileDeleted)
        {
            return false;
        }

        await repository.DeleteAsync(document);
        await unitOfWork.SaveAsync();
        return true;
    }

    public async Task<JobSeekerDocumentDto> UploadDocumentAsync(FileUpload file, string type, string email)
    {
        if (!Enum.TryParse<DocumentType>(type, true, out var documentType))
        {
            throw new ArgumentException("Invalid document type.");
        }

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }

        var userInDb = await unitOfWork.UserRepository.GetByEmailAsync(email);
        if (userInDb == null)
        {
            throw new NotFoundException("User not found.");
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

        var downloadUrl = await fileStorageService.UploadFileAsync(file.Content, uniqueFileName, file.ContentType);

        var createRequest = new CreateJobSeekerDocumentRequest(userInDb.Id, downloadUrl, documentType);
        var document = await CreateAsync(createRequest);
        return document;
    }
}