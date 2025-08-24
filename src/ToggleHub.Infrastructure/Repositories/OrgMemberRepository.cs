using Microsoft.EntityFrameworkCore;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

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

    public async Task<IEnumerable<OrgMember>> GetMembersInOrganizationAsync(int organizationId)
    {
        return await _context.OrgMembers
            .Where(om => om.OrgId == organizationId)
            .ToListAsync();
    }


    public async Task<bool> IsUserInOrganizationAsync(int organizationId, int userId)
    {
        return await _context.OrgMembers
            .AnyAsync(om => om.OrgId == organizationId && om.UserId == userId);
    }

    public async Task<OrgMember?> GetOrgMemberAsync(int organizationId, int userId)
    {
        return await _context.OrgMembers
            .FirstOrDefaultAsync(om => om.OrgId == organizationId && om.UserId == userId);
    }
}
