// Pagination Configuration
export const PAGINATION_CONFIG = {
	DEFAULT_PAGE: 0,
	DEFAULT_PAGE_SIZE: 25,
	PAGE_SIZE_OPTIONS: [5, 10, 25, 50]
};

// Invite Status Constants
export const INVITE_STATUS = {
	PENDING: 'Pending',
	ACCEPTED: 'Accepted',
	DECLINED: 'Declined',
	EXPIRED: 'Expired'
};

// Organization Role Constants
export const ORG_ROLES = {
	OWNER: 'Owner',
	ADMIN: 'Admin',
	FLAG_MANAGER: 'FlagManager'
};

// Chip Color Mapping
export const CHIP_COLORS = {
	[INVITE_STATUS.PENDING]: 'warning',
	[INVITE_STATUS.ACCEPTED]: 'success',
	[INVITE_STATUS.DECLINED]: 'error',
	[INVITE_STATUS.EXPIRED]: 'error',
	[ORG_ROLES.OWNER]: 'primary',
	[ORG_ROLES.ADMIN]: 'secondary',
	[ORG_ROLES.FLAG_MANAGER]: 'info'
};

// User Messages
export const MESSAGES = {
	SUCCESS: {
		INVITATION_SENT: (email) => `Invitation sent to ${email}`,
		INVITATION_REVOKED: 'Invitation revoked successfully',
		INVITATION_RESENT: 'Invitation resent successfully',
		ROLE_UPDATED: (email, role) => `Role for ${email} updated to ${role}`
	},
	ERROR: {
		SERVER_ERROR: 'An internal server error occurred. Please try again later.',
		INVITATION_FAILED: 'Failed to send invitation',
		REVOKE_FAILED: 'Failed to revoke invitation',
		RESEND_FAILED: 'Failed to resend invitation',
		ROLE_UPDATE_FAILED: 'Failed to update role',
		ORGANIZATION_NOT_FOUND: 'Organization not found',
		MEMBERS_LOAD_FAILED: 'Failed to load members',
		INVITES_LOAD_FAILED: 'Failed to load invites'
	}
};
