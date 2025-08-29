using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;
using ToggleHub.Infrastructure.Extensions;

namespace ToggleHub.Infrastructure.Repositories;

public class OrgMemberRepository : BaseRepository<OrgMember>, IOrgMemberRepository
{
    public OrgMemberRepository(ToggleHubDbContext context) : base(context)
    {
    }
    public async Task AddOrgMemberAsync(OrgMember orgMember)
    {
        _context.OrgMembers.Add(orgMember);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrgMember(OrgMember orgMember)
    {
        _context.OrgMembers.Remove(orgMember);
        await _context.SaveChangesAsync();
    }

    public async Task<IPagedList<OrgMember>> GetMembersInOrganizationAsync(int organizationId, int pageIndex = 0, int pageSize = Int32.MaxValue)
    {
        return await _context.OrgMembers
            .Where(om => om.OrganizationId == organizationId)
            .ToPagedListAsync(pageIndex, pageSize);
    }
    


    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await _context.OrgMembers
            .AnyAsync(om => om.OrganizationId == organizationId && om.UserId == userId);
    }

    public async Task<OrgMember?> GetOrgMemberAsync(int organizationId, int userId)
    {
        return await _context.OrgMembers
            .FirstOrDefaultAsync(om => om.OrganizationId == organizationId && om.UserId == userId);
    }
}
