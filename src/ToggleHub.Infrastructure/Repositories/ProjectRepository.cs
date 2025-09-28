using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class ProjectRepository : BaseSluggedRepository<Project>, IProjectRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;

    public ProjectRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
    }

    protected override IQueryable<Project> WithIncludes(DbSet<Project> dbSet)
    {
        return dbSet
                    .Include(p => p.Environments);
    }
    public async Task<bool> NameExistsAsync(string name, int organizationId = 0)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && (organizationId == 0 || o.OrganizationId == organizationId));
    }

    public async Task<IPagedList<Project>> GetAllAsync(int? organizationId = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var cacheKey = _cacheKeyFactory.For<Project>(new Dictionary<string, object?>
        {
            { nameof(organizationId), organizationId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _dbSet.AsQueryable();

            if (organizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == organizationId.Value);
            }

            query = query.OrderByDescending(x => x.Id);
            return await query.ToPagedListAsync(pageIndex, pageSize);
        });
    }

    public async Task<Project?> GetBySlugAsync(string slug, int organizationId)
    {
        var cacheKey = _cacheKeyFactory.For<Project>(new Dictionary<string, object?>
        {
            { nameof(slug), slug },
            { nameof(organizationId), organizationId }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await WithIncludes(_dbSet)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Slug == slug && o.OrganizationId == organizationId);
        });
    }
}
