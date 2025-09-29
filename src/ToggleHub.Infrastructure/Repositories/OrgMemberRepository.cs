using Microsoft.EntityFrameworkCore;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class OrgMemberRepository : BaseRepository<OrgMember>, IOrgMemberRepository
{
    private readonly ICacheManager _cacheManager;
    private readonly ICacheKeyFactory _cacheKeyFactory;


    public OrgMemberRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory, IEventPublisher eventPublisher) : base(context, cacheManager, cacheKeyFactory, eventPublisher)
    {
        _cacheManager = cacheManager;
        _cacheKeyFactory = cacheKeyFactory;
    }

    public async Task<IPagedList<OrgMember>> GetMembersInOrganizationAsync(int organizationId, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        var cacheKey = _cacheKeyFactory.For<OrgMember>(new Dictionary<string, object?>
        {
            { nameof(organizationId), organizationId },
            { nameof(pageIndex), pageIndex },
            { nameof(pageSize), pageSize }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await _context.OrgMembers
                .Where(om => om.OrganizationId == organizationId)
                .OrderByDescending(x => x.Id)
                .ToPagedListAsync(pageIndex, pageSize);
        });
    }
    


    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await GetOrgMemberAsync(organizationId, userId) != null;
    }

    public async Task<OrgMember?> GetOrgMemberAsync(int organizationId, int userId)
    {
        var cacheKey = _cacheKeyFactory.For<OrgMember>(new Dictionary<string, object?>
        {
            { nameof(organizationId), organizationId },
            { nameof(userId), userId }
        });

        return await _cacheManager.GetAsync(cacheKey, async () =>
        {
            return await _context.OrgMembers
                .FirstOrDefaultAsync(om => om.OrganizationId == organizationId && om.UserId == userId);
        });
    }
}
