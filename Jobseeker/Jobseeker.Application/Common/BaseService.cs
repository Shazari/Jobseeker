using Jobseeker.Domain.Common;
using Serilog;

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
        Log.Information("Creating a new entity of type {EntityType}", typeof(TEntity).Name);
        var entity = MapToEntity(createDto);
        await repository.AddAsync(entity);
        await unitOfWork.SaveAsync();
        Log.Information("Entity created successfully with ID: {EntityId}", entity.Id);
        return MapToDto(entity);
    }

    public virtual async Task<TDto?> GetByIdAsync(Guid id)
    {
        Log.Information("Fetching entity of type {EntityType} with ID: {EntityId}", typeof(TEntity).Name, id);
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            Log.Warning("Entity with ID: {EntityId} not found", id);
            return null;
        }
        Log.Information("Entity retrieved successfully with ID: {EntityId}", id);
        return MapToDto(entity);
    }

    public virtual async Task<IList<TDto>> GetAllAsync()
    {
        Log.Information("Fetching all entities of type {EntityType}", typeof(TEntity).Name);
        var list = await repository.GetAllAsync();
        Log.Information("Retrieved {Count} entities of type {EntityType}", list.Count, typeof(TEntity).Name);
        return list.Select(MapToDto).ToList();
    }

    public virtual async Task<TDto?> UpdateAsync(TUpdateDto updateDto)
    {
        var entityId = GetEntityIdFromUpdateDto(updateDto);
        Log.Information("Attempting to update entity of type {EntityType} with ID: {EntityId}", typeof(TEntity).Name, entityId);

        var existingEntity = await repository.GetByIdAsync(entityId);
        if (existingEntity is null)
        {
            Log.Warning("Entity with ID: {EntityId} not found, update failed", entityId);
            return null;
        }

        MapToExistingEntity(updateDto, existingEntity);
        await repository.UpdateAsync(existingEntity);
        await unitOfWork.SaveAsync();

        Log.Information("Entity of type {EntityType} with ID: {EntityId} updated successfully", typeof(TEntity).Name, entityId);
        return MapToDto(existingEntity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        Log.Information("Attempting to delete entity of type {EntityType} with ID: {EntityId}", typeof(TEntity).Name, id);
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            Log.Warning("Entity with ID: {EntityId} not found, deletion aborted", id);
            return;
        }

        await repository.DeleteAsync(entity);
        await unitOfWork.SaveAsync();
        Log.Information("Entity of type {EntityType} with ID: {EntityId} deleted successfully", typeof(TEntity).Name, id);
    }
}
