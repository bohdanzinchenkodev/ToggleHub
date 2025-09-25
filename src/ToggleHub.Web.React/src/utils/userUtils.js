/**
 * Gets initials from user data in a consistent way across the app
 * Priority: firstName + lastName > userName > email
 */
export const getUserInitials = (user) => {
	if (!user) return 'U';
	
	// Try firstName + lastName first
	const firstName = user.firstName?.trim();
	const lastName = user.lastName?.trim();
	
	if (firstName && lastName) {
		return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
	}
	
	if (firstName) {
		return firstName.charAt(0).toUpperCase();
	}
	
	// Fall back to userName
	if (user.userName?.trim()) {
		return user.userName
			.split(' ')
			.map(word => word.charAt(0))
			.join('')
			.toUpperCase()
			.slice(0, 2);
	}
	
	// Final fallback to email
	if (user.email?.trim()) {
		return user.email.charAt(0).toUpperCase();
	}
	
	return 'U';
};

/**
 * Gets display name from user data
 * Priority: firstName lastName > userName > email
 */
export const getUserDisplayName = (user) => {
	if (!user) return 'User';
	
	const firstName = user.firstName?.trim();
	const lastName = user.lastName?.trim();
	
	if (firstName && lastName) {
		return `${firstName} ${lastName}`;
	}
	
	if (firstName) {
		return firstName;
	}
	
	if (user.userName?.trim()) {
		return user.userName;
	}
	
	if (user.email?.trim()) {
		return user.email;
	}
	
	return 'User';
};
