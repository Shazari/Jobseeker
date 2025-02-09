using Jobseeker.Domain.Common;
using Jobseeker.Domain.Interfaces;
using Jobseeker.Infrastructure.Data.Repositories;

namespace Jobseeker.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _databaseContext;

    public bool IsDisposed { get; protected set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _databaseContext = context;
    }

    public async Task SaveAsync() => await _databaseContext.SaveChangesAsync();

    private IUserRepository? _userRepository;

    public IUserRepository UserRepository
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository =
                    new UserRepository(_databaseContext);
            }

            return _userRepository;
        }
    }

    private IJobPostRepository? _jobPostRepository;

    public IJobPostRepository JobPostRepository
    {
        get
        {
            if (_jobPostRepository == null)
            {
                _jobPostRepository =
                    new JobPostRepository(_databaseContext);
            }

            return _jobPostRepository;
        }
    }

    private IJobApplicationRepository? _jobApplicationRepository;

    public IJobApplicationRepository JobApplicationRepository
    {
        get
        {
            if (_jobApplicationRepository == null)
            {
                _jobApplicationRepository =
                    new JobApplicationRepository(_databaseContext);
            }

            return _jobApplicationRepository;
        }
    }

    private IDocumentRepository? _documentRepository;

    public IDocumentRepository DocumentRepository
    {
        get
        {
            if (_documentRepository == null)
            {
                _documentRepository =
                    new DocumentRepository(_databaseContext);
            }

            return _documentRepository;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _databaseContext.Dispose();
            }
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
