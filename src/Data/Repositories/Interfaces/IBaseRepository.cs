using FunnyExperience.Server.Models.DatabaseModels.Base;

namespace FunnyExperience.Server.Data.Repositories.Interfaces;

public interface IBaseRepository<Entity> where Entity : BaseEntity {
    Task<List<Entity?>> AllAsync();
    Task<IEnumerable<TDto>> AllAsync<TDto>();
    Task<Guid> AddAsync(Entity entity);
    Task<TDto> AddAsync<TDto>(Entity entity);
    Task AddMultipleAsync(IEnumerable<Entity> entities);
    Task<TDto> UpdateAsync<TDto>(Guid id, Entity entity);
    Task<TDto> UpdateAsync<TDto>(Entity entity);
    Task UpdateAsync(Guid id, Entity entity);
    Task<Entity?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entity>> GetByIdsAsync(List<Guid> ids);
    Task<TDto> GetByIdAsync<TDto>(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<TDto>> GetByIdsAsync<TDto>(List<Guid> ids);
    Task<Entity> GetByIdWithoutTrackingAsync(Guid id);
    Task UpdatePartialAsync(Entity? entity);
    Task BulkUpdateAsync(IEnumerable<Entity> entities);
}