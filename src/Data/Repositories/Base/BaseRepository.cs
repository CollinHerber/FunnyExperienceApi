using AutoMapper;
using AutoMapper.QueryableExtensions;
using FunnyExperience.Server.Data.Extensions;
using FunnyExperience.Server.Data.Repositories.Interfaces;
using FunnyExperience.Server.Models.DatabaseModels.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FunnyExperience.Server.Data.Repositories.Base;

public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : BaseEntity
{
    private readonly IMapper _mapper;

    protected readonly FunnyExperienceDbContext Db;
    protected DbSet<Entity?> Table => Db.Set<Entity>();

    public virtual string EntityName { get; protected set; }

    public BaseRepository(FunnyExperienceDbContext db, IMapper mapper)
    {
        Db = db;
        _mapper = mapper;
    }

    public async virtual Task<List<Entity?>> AllAsync()
    {
        return await Table.ToListAsync();
    }

    public async virtual Task<IEnumerable<TDto>> AllAsync<TDto>()
    {
        return await Table.ProjectTo<TDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async virtual Task<TDto> AddAsync<TDto>(Entity entity)
    {
        await AddAsync(entity);
        return _mapper.Map<TDto>(entity);
    }

    public async virtual Task<Guid> AddAsync(Entity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.UpdatedDate = entity.CreatedDate;
        CreatePropertyIgnore(entity);
        Db.Add(entity);
        await Db.SaveChangesAsync();
        return entity.Id;
    }

    public async virtual Task AddMultipleAsync(IEnumerable<Entity> entities)
    {
        var date = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            entity.CreatedDate = date;
            entity.UpdatedDate = date;
            CreatePropertyIgnore(entity);
        }
        await Db.AddRangeAsync(entities);
        await Db.SaveChangesAsync();
    }

    public async virtual Task<Entity?> GetByIdAsync(Guid id)
    {
        return await Table.FindAsync(id);
    }

    public async virtual Task<Entity> GetByIdWithoutTrackingAsync(Guid id)
    {
        return await Table.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }

    public async virtual Task<IEnumerable<Entity>> GetByIdsAsync(List<Guid> ids)
    {
        return await Table.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async virtual Task<IEnumerable<TDto>> GetByIdsAsync<TDto>(List<Guid> ids)
    {
        return await Table.Where(x => ids.Contains(x.Id)).ProjectTo<TDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async virtual Task<TDto> GetByIdAsync<TDto>(Guid id)
    {
        return await Table.Where(x => x.Id == id).ProjectTo<TDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async virtual Task<bool> ExistsAsync(Guid id)
    {
        return await Table.AnyAsync(x => x.Id == id);
    }

    public async virtual Task<TDto> UpdateAsync<TDto>(Guid id, Entity? entity)
    {
        await UpdateAsync(id, entity);
        return _mapper.Map<TDto>(entity);
    }

    public async virtual Task<TDto> UpdateAsync<TDto>(Entity entity)
    {
        await UpdateAsync(entity.Id, entity);
        return _mapper.Map<TDto>(entity);
    }

    public async virtual Task UpdateAsync(Guid id, Entity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        entity.Id = id;
        var entry = Table.Update(entity);
        entry.Property(x => x.CreatedDate).IsModified = false;
        UpdatePropertyIgnore(entity);
        await Db.SaveChangesAsync();
    }

    public async virtual Task BulkUpdateAsync(IEnumerable<Entity> entities)
    {
        var date = DateTime.UtcNow;

        foreach (var entity in entities)
        {
            entity.UpdatedDate = date;
            CreatePropertyIgnore(entity);
        }
        Db.UpdateRange(entities);
        await Db.SaveChangesAsync();
    }

    public async virtual Task UpdatePartialAsync(Entity? entity)
    {
        await UpdatePartialAsync(entity.Id, entity);
    }

    private async Task UpdatePartialAsync(Guid id, Entity? entity)
    {
        var type = typeof(Entity).GetPropertiesFor();
        entity.UpdatedDate = DateTime.UtcNow;
        entity.Id = id;
        var entry = Table.Attach(entity);

        entry.Properties.Where(y => !y.Metadata.IsKey()).ToList().ForEach(x =>
        {
            var isDefaultValueOfType = TypeExtensions.IsDefaultValue(x.GetType(), x.CurrentValue);
            x.IsModified = !isDefaultValueOfType;
        });

        entry.Property(x => x.CreatedDate).IsModified = false;
        UpdatePropertyIgnore(entity);
        await Db.SaveChangesAsync();
    }

    protected void UpdatePropertyIgnore(Entity? entity) { }
    protected void CreatePropertyIgnore(Entity entity) { }


}