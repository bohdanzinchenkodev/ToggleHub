import { VALIDATION_MESSAGES, VALIDATION_RULES } from '../constants/validationConstants';

/**
 * Validates that a value is not empty
 * @param {string} value - Value to validate
 * @param {string} fieldName - Field name for error message
 * @returns {string|null} Error message or null if valid
 */
export const validateRequired = (value, fieldName) => {
	if (!value || !value.toString().trim()) {
		return VALIDATION_MESSAGES.REQUIRED(fieldName);
	}
	return null;
};

/**
 * Validates an email address format
 * @param {string} email - Email to validate
 * @returns {string|null} Error message or null if valid
 */
export const validateEmail = (email) => {
	if (!email || !email.trim()) {
		return VALIDATION_MESSAGES.EMAIL.REQUIRED;
	}
	if (!VALIDATION_RULES.EMAIL_REGEX.test(email.trim())) {
		return VALIDATION_MESSAGES.EMAIL.INVALID;
	}
	return null;
};

/**
 * Validates password strength
 * @param {string} password - Password to validate
 * @param {number} minLength - Minimum password length
 * @returns {string|null} Error message or null if valid
 */
export const validatePassword = (password, minLength = VALIDATION_RULES.PASSWORD.MIN_LENGTH) => {
	if (!password) {
		return VALIDATION_MESSAGES.PASSWORD.REQUIRED;
	}
	if (password.length < minLength) {
		return VALIDATION_MESSAGES.PASSWORD.MIN_LENGTH(minLength);
	}
	return null;
};

/**
 * Validates password confirmation matches
 * @param {string} password - Original password
 * @param {string} confirmPassword - Confirmation password
 * @returns {string|null} Error message or null if valid
 */
export const validatePasswordMatch = (password, confirmPassword) => {
	if (!confirmPassword) {
		return VALIDATION_MESSAGES.PASSWORD.REQUIRED;
	}
	if (password !== confirmPassword) {
		return VALIDATION_MESSAGES.PASSWORD.NO_MATCH;
	}
	return null;
};

/**
 * Validates form data against rules
 * @param {object} data - Form data to validate
 * @param {object} rules - Validation rules
 * @returns {object} Object with field errors
 */
export const validateForm = (data, rules) => {
	const errors = {};
	
	Object.keys(rules).forEach(field => {
		const rule = rules[field];
		if (rule.required) {
			const error = validateRequired(data[field], rule.label || field);
			if (error) {
				errors[field] = error;
			}
		}
		if (rule.email) {
			const error = validateEmail(data[field]);
			if (error) {
				errors[field] = error;
			}
		}
		if (rule.password) {
			const error = validatePassword(data[field], rule.minLength);
			if (error) {
				errors[field] = error;
			}
		}
	});
	
	return errors;
};
