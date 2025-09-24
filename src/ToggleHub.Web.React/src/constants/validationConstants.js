/**
 * Validation constants and error messages
 */

export const VALIDATION_MESSAGES = {
	REQUIRED: (fieldName) => `${fieldName} is required`,
	EMAIL: {
		REQUIRED: 'Email is required',
		INVALID: 'Invalid email format'
	},
	PASSWORD: {
		REQUIRED: 'Password is required',
		MIN_LENGTH: (minLength) => `Password must be at least ${minLength} characters`,
		NO_MATCH: 'Passwords do not match'
	},
	GENERAL: {
		REQUIRED: 'This field is required',
		INVALID_FORMAT: 'Invalid format'
	}
};

export const VALIDATION_RULES = {
	EMAIL_REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
	PASSWORD: {
		MIN_LENGTH: 6
	}
};
