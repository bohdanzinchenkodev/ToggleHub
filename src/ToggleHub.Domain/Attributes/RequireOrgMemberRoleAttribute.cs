using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class RequireOrgMemberRoleAttribute : Attribute
{
    public OrgMemberRole[] RequiredRoles { get; }
    public RequireOrgMemberRoleAttribute(params OrgMemberRole[] requiredRoles)
    {
        RequiredRoles = requiredRoles;
    }
}
