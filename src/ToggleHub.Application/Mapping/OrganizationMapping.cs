using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class OrganizationMapping
{
    public static OrganizationDto ToDto(this Organization organization)
    {
        return new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Slug = organization.Slug
        };
    }
    public static Organization ToEntity(this CreateOrganizationDto createDto, Organization? organization = null)
    {
        organization ??= new Organization();
        organization.Name = createDto.Name;
        return organization;
    }
    
    public static Organization ToEntity(this UpdateOrganizationDto updateDto, Organization? organization = null)
    {
        organization ??= new Organization();
        organization.Id = updateDto.Id;
        organization.Name = updateDto.Name;
        return organization;
    }
    
    public static OrgMemberDto ToDto(this OrgMember orgMember, UserDto userDto)
    {
        return new OrgMemberDto
        {
            User = userDto,
            Id = orgMember.Id,
            OrganizationId = orgMember.OrganizationId,
            OrganizationRole = orgMember.Role
        };
    }
}