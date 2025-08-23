using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class OrgMemberRepository : BaseRepository<OrgMember>, IOrgMemberRepository
{
    public OrgMemberRepository(ToggleHubDbContext context) : base(context)
    {
    }
}
