using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ToggleHubDbContext context) : base(context)
    {
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExistsAsync(string name, int excludeId)
    {
        return await _dbSet
            .AnyAsync(o => o.Name.ToLower() == name.ToLower() && o.Id != excludeId);
    }

    public async Task<IPagedList<Organization>> GetOrganizationsByUserIdAsync(int userId, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var query = _context.OrgMembers
            .Include(x => x.Organization)
            .Where(x => x.UserId == userId)
            .Select(x => x.Organization)
            .Distinct();
        
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }
}
