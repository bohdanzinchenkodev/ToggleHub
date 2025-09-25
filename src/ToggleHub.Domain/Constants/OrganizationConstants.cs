using ToggleHub.Domain.Attributes;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Constants;

public static class OrganizationConstants
{
    public static class OrganizationPermissions
    {
        [RequireOrgMemberRole(OrgMemberRole.FlagManager, OrgMemberRole.Admin, OrgMemberRole.Owner)]
        public const string ViewOrganization = "view_organization";
        
        [RequireOrgMemberRole(OrgMemberRole.Admin, OrgMemberRole.Owner)]
        public const string EditOrganization = "edit_organization";
        
        [RequireOrgMemberRole(OrgMemberRole.Owner)]
        public const string DeleteOrganization = "delete_organization";
        
        [RequireOrgMemberRole(OrgMemberRole.Admin, OrgMemberRole.Owner)]
        public const string ManageMembers = "manage_members";
        
        [RequireOrgMemberRole(OrgMemberRole.Admin, OrgMemberRole.Owner)]
        public const string ManageProjects = "manage_projects";
        [RequireOrgMemberRole(OrgMemberRole.Admin, OrgMemberRole.Owner, OrgMemberRole.FlagManager)]
        public const string ViewProjects = "view_projects";
        
        [RequireOrgMemberRole(OrgMemberRole.FlagManager, OrgMemberRole.Admin, OrgMemberRole.Owner)]
        public const string ManageFlags = "manage_features";
    }
}