using Jobseeker.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Jobseeker.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    internal ApplicationDbContext DatabaseContext { get; }

    internal DbSet<T> DbSet { get; }

    public Repository(ApplicationDbContext databaseContext) : base()
    {
        DatabaseContext =
            databaseContext ?? throw new ArgumentNullException(paramName: nameof(databaseContext));
        
        DbSet = DatabaseContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(keyValues: id);
    }

    public async Task<IList<T>> GetAllAsync()
    {
        var result =
                await
                DbSet.ToListAsync()
                ;

        return result;
    }

    public async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(paramName: nameof(entity));
        }

        await DbSet.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(paramName: nameof(entity));
        }

        await Task.Run(() =>
        {
            DbSet.Update(entity);
        });
    }

    public async Task DeleteAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(paramName: nameof(entity));
        }

        await Task.Run(() =>
        {
            DbSet.Remove(entity);
        });
    }
}
