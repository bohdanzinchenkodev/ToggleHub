using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationService : BaseService<Organization>
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository) 
        : base(organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }
}
