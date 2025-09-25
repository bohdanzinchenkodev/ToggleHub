// User permission constants
// These correspond to the backend permission constants
export const PERMISSIONS = {
	VIEW_ORGANIZATION: "view_organization",
	EDIT_ORGANIZATION: "edit_organization", 
	DELETE_ORGANIZATION: "delete_organization",
	MANAGE_MEMBERS: "manage_members",
	MANAGE_PROJECTS: "manage_projects",
    VIEW_PROJECTS: "view_projects",
	MANAGE_FLAGS: "manage_features",
    
};

// Helper function to check if user has a specific permission
export const hasPermission = (userPermissions, permission) => {
	return userPermissions && userPermissions.includes(permission);
};

// Helper function to check if user has any of the specified permissions
export const hasAnyPermission = (userPermissions, permissions) => {
	if (!userPermissions || !Array.isArray(permissions)) return false;
	return permissions.some(permission => userPermissions.includes(permission));
};

// Helper function to check if user has all of the specified permissions
export const hasAllPermissions = (userPermissions, permissions) => {
	if (!userPermissions || !Array.isArray(permissions)) return false;
	return permissions.every(permission => userPermissions.includes(permission));
};
