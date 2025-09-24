import { MESSAGES, CHIP_COLORS } from '../constants/organizationConstants';

/**
 * Validates an email address
 * @param {string} email - Email to validate
 * @returns {string} - Error message if invalid, empty string if valid
 */
export const validateEmail = (email) => {
	const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
	if (!email.trim()) {
		return MESSAGES.VALIDATION.EMAIL_REQUIRED;
	}
	if (!emailRegex.test(email.trim())) {
		return MESSAGES.VALIDATION.EMAIL_INVALID;
	}
	return '';
};

/**
 * Gets the appropriate chip color for a given value
 * @param {string} value - The value to get color for (role or status)
 * @returns {string} - Material-UI color name
 */
export const getChipColor = (value) => CHIP_COLORS[value] || 'default';

/**
 * Formats a date string to locale date string
 * @param {string} dateString - ISO date string
 * @returns {string} - Formatted date string
 */
export const formatDate = (dateString) => new Date(dateString).toLocaleDateString();
