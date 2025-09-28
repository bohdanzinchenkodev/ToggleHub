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
    public ProjectRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }

    public async Task<bool> NameExistsAsync(string name, int organizationId = 0)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && (organizationId == 0 || o.OrganizationId == organizationId));
    }

    public async Task<IPagedList<Project>> GetAllAsync(int? organizationId = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var query = _dbSet.AsQueryable();

        if (organizationId.HasValue)
        {
            query = query.Where(p => p.OrganizationId == organizationId.Value);
        }

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public Task<Project?> GetBySlugAsync(string slug, int organizationId)
    {
        return _dbSet
            .AsNoTracking()
            .Include(x => x.Environments)
            .FirstOrDefaultAsync(o => o.Slug == slug && o.OrganizationId == organizationId);
    }
}
