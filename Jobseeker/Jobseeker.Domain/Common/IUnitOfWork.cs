using Jobseeker.Domain.Interfaces;

namespace Jobseeker.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    bool IsDisposed { get; }

    Task SaveAsync();

    IRoleRepository RoleRepository { get; }
    IUserRepository UserRepository { get; }
    IDocumentRepository DocumentRepository { get; }
    IJobApplicationRepository JobApplicationRepository { get; }
    IJobPostRepository JobPostRepository { get; }
}
