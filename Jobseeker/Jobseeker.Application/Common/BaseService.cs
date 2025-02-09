using Jobseeker.Domain.Common;

namespace Jobseeker.Application.Common;

public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    (IRepository<TEntity> repository, IUnitOfWork unitOfWork)
    : IBaseService<TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
    where TDto : class
{
    // Abstract mappers (domain <-> Dto)
    protected abstract TDto MapToDto(TEntity entity);
    protected abstract TEntity MapToEntity(TCreateDto createDto);
    protected abstract void MapToExistingEntity(TUpdateDto updateDto, TEntity entity);
    protected abstract Guid GetEntityIdFromUpdateDto(TUpdateDto updateDto);

    // CRUD methods from IBaseService
    public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
    {
        var entity = MapToEntity(createDto);
        await repository.AddAsync(entity);
        await unitOfWork.SaveAsync();
        return MapToDto(entity);
    }

    public virtual async Task<TDto?> GetByIdAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id);
        return entity is null ? null : MapToDto(entity);
    }

    public virtual async Task<IList<TDto>> GetAllAsync()
    {
        var list = await repository.GetAllAsync();
        return list.Select(MapToDto).ToList();
    }

    public virtual async Task<TDto?> UpdateAsync(TUpdateDto updateDto)
    {
        var entityId = GetEntityIdFromUpdateDto(updateDto);

        var existingEntity = await repository.GetByIdAsync(entityId);
        if (existingEntity is null)
            return null;

        MapToExistingEntity(updateDto, existingEntity);

        await repository.UpdateAsync(existingEntity);
        await unitOfWork.SaveAsync();

        return MapToDto(existingEntity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null) return;
        await repository.DeleteAsync(entity);
        await unitOfWork.SaveAsync();
    }
}