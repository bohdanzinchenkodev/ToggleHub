using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ToggleHubDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly ICacheManager _cacheManager;
    private readonly IRepositoryCacheKeyFactory _cacheKeyFactory;

    public BaseRepository(
        ToggleHubDbContext context,
        ICacheManager cacheManager,
        IRepositoryCacheKeyFactory cacheKeyFactory)
    {
        _context = context;
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
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
                var query = WithIncludes(_dbSet).AsNoTracking();
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
                var query = WithIncludes(_dbSet).AsNoTracking();
                return query.ToPagedListAsync(pageIndex, pageSize);
            });
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Invalidate list caches
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());

        return entity;
    }

    public async Task CreateAsync(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        // Evict specific + related caches
        await _cacheManager.RemoveAsync(_cacheKeyFactory.ForEntityById<T>(entity.Id).Key);
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
    }

    public async Task UpdateAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if(entity == null)
            return;
        
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();

        await _cacheManager.RemoveAsync(_cacheKeyFactory.ForEntityById<T>(id).Key);
        await _cacheManager.RemoveByPrefixAsync(_cacheKeyFactory.PrefixForEntity<T>());
    }
}
