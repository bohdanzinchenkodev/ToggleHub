using System.Reflection;
using ToggleHub.Domain.Attributes;
using ToggleHub.Domain.Constants;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Domain.Helpers;

public static class PermissionRoleHelper
{
    public static OrgMemberRole[] GetRolesByPermission(string permission)
    {
        var orgPermissionField = typeof(OrganizationConstants.OrganizationPermissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(f => f.GetValue(null)?.ToString() == permission);

        if (orgPermissionField != null)
        {
            var attribute = orgPermissionField.GetCustomAttribute<RequireOrgMemberRoleAttribute>();
            return attribute?.RequiredRoles ?? Array.Empty<OrgMemberRole>();
        }
        
        return new[] { OrgMemberRole.Owner };
    }
    
    public static string[] GetPermissionsForRole(OrgMemberRole role)
    {
        var permissions = new List<string>();
        
        var orgPermissionFields = typeof(OrganizationConstants.OrganizationPermissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var field in orgPermissionFields)
        {
            var attribute = field.GetCustomAttribute<RequireOrgMemberRoleAttribute>();
            if (attribute?.RequiredRoles.Contains(role) == true)
            {
                var permission = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(permission))
                {
                    permissions.Add(permission);
                }
            }
        }

        return permissions.ToArray();
    }
}
