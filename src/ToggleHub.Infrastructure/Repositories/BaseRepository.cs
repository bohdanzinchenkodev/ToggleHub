using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ToggleHubDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;
    private readonly IEventPublisher _eventPublisher;

    public BaseRepository(
        ToggleHubDbContext context,
        ICacheManager cacheManager,
        ICacheKeyFactory cacheKeyFactory, IEventPublisher eventPublisher)
    {
        _context = context;
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
        _eventPublisher = eventPublisher;
        _dbSet = context.Set<T>();
    }
    

    // Override in derived repositories to apply eager-loading Includes.
    protected virtual IQueryable<T> WithIncludes(DbSet<T> dbSet)
    {
        return dbSet; // default = no includes
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        var key = _cacheKeyFactory.ForEntityById<T>(id);

        return await _cacheManager.GetAsync(
            key,
            () =>
            {
                var query = WithIncludes(_dbSet);
                return query.FirstOrDefaultAsync(x => x.Id == id)!;
            });
    }

    public virtual Task<IPagedList<T>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var key = _cacheKeyFactory.ForEntityAll<T>(pageIndex, pageSize);

        return _cacheManager.GetAsync(
            key,
            () =>
            {
                var query = WithIncludes(_dbSet)
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id);
                return query.ToPagedListAsync(pageIndex, pageSize);
            });
    }

    public virtual async Task<T> CreateAsync(T entity, bool publishEvents = true)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Invalidate list caches
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());

        if (publishEvents)
        {
            var eventMessage = new EntityCreatedEvent<T>(entity);
            await _eventPublisher.PublishAsync(eventMessage);
        }

        return entity;
    }

    public async Task CreateAsync(IEnumerable<T> entities, bool publishEvents = true)
    {
        var entityList = entities.ToList();
        _dbSet.AddRange(entityList);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
        
        if (publishEvents)
        {
            var eventMessage = new EntitiesCreatedEvent<T>(entityList.ToList());
            await _eventPublisher.PublishAsync(eventMessage);
        }
    }

    public virtual async Task UpdateAsync(T entity, bool publishEvents = true)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        // Evict specific + related caches
        await _cacheManager.RemoveAsync(_cacheKeyFactory.ForEntityById<T>(entity.Id).Key);
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
        
        if (publishEvents)
        {
            var eventMessage = new EntityUpdatedEvent<T>(entity);
            await _eventPublisher.PublishAsync(eventMessage);
        }
    }

    public async Task UpdateAsync(IEnumerable<T> entities, bool publishEvents = true)
    {
        var entityList = entities.ToList();
        _dbSet.UpdateRange(entityList);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());

        if (publishEvents)
        {
            var eventMessage = new EntitiesUpdatedEvent<T>(entityList);
            await _eventPublisher.PublishAsync(eventMessage);
        }
        
    }

    public virtual async Task DeleteAsync(int id, bool publishEvents = true)
    {
        var entity = await _dbSet.FindAsync(id);
        if(entity == null)
            return;
        
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveAsync(_cacheKeyFactory.ForEntityById<T>(id).Key);
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
        
        if (publishEvents)
        {
            var eventMessage = new EntityDeletedEvent<T>(entity);
            await _eventPublisher.PublishAsync(eventMessage);
        }
    }
}
