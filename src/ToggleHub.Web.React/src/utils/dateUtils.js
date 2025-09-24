/**
 * Formats a date string to locale date string
 * @param {string} dateString - ISO date string
 * @param {string} locale - Locale for formatting (default: user's locale)
 * @param {object} options - Intl.DateTimeFormat options
 * @returns {string} Formatted date string
 */
export const formatDate = (dateString, locale = undefined, options = {}) => {
	if (!dateString) return '';
	
	const defaultOptions = {
		year: 'numeric',
		month: 'short',
		day: 'numeric',
		...options
	};
	
	return new Date(dateString).toLocaleDateString(locale, defaultOptions);
};

/**
 * Formats a date to relative time (e.g., "2 days ago")
 * @param {string} dateString - ISO date string
 * @returns {string} Relative time string
 */
export const formatRelativeTime = (dateString) => {
	if (!dateString) return '';
	
	const date = new Date(dateString);
	const now = new Date();
	const diffInSeconds = Math.floor((now - date) / 1000);
	
	if (diffInSeconds < 60) return 'Just now';
	if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minutes ago`;
	if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hours ago`;
	if (diffInSeconds < 2592000) return `${Math.floor(diffInSeconds / 86400)} days ago`;
	
	return formatDate(dateString);
};

/**
 * Checks if a date is expired
 * @param {string} dateString - ISO date string
 * @returns {boolean} True if date is in the past
 */
export const isExpired = (dateString) => {
	if (!dateString) return false;
	return new Date(dateString) < new Date();
};
