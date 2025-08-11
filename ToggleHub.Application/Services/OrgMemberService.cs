using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrgMemberService : BaseService<OrgMember>
{
    private readonly IOrgMemberRepository _orgMemberRepository;

    public OrgMemberService(IOrgMemberRepository orgMemberRepository) 
        : base(orgMemberRepository)
    {
        _orgMemberRepository = orgMemberRepository;
    }
}
