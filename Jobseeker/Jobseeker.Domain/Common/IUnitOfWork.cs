namespace Jobseeker.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    bool IsDisposed { get; }

    Task SaveAsync();
}
