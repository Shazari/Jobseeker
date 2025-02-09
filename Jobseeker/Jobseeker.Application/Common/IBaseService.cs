namespace Jobseeker.Application.Common;

public interface IBaseService<TDto, in TCreateDto, in TUpdateDto>
{
    Task<TDto> CreateAsync(TCreateDto createDto);
    Task<TDto?> GetByIdAsync(Guid id);
    Task<IList<TDto>> GetAllAsync();
    Task<TDto?> UpdateAsync(TUpdateDto updateDto);
    Task DeleteAsync(Guid id);
}