/**
 * API utility functions for common operations
 */

/**
 * Handles API errors and returns a formatted error message
 * @param {object} error - RTK Query error object
 * @returns {string} Formatted error message
 */
export const getErrorMessage = (error) => {
	if (error?.data?.message) {
		return error.data.message;
	}
	if (error?.data?.errors && Array.isArray(error.data.errors)) {
		return error.data.errors.join(', ');
	}
	if (error?.message) {
		return error.message;
	}
	return 'An unexpected error occurred';
};

/**
 * Transforms API error to form validation errors
 * @param {object} error - RTK Query error object
 * @returns {object} Object with field names as keys and error messages as values
 */
export const transformApiErrorToValidation = (error) => {
	const validationErrors = {};
	
	if (error?.data?.errors) {
		// Handle validation errors from API
		if (typeof error.data.errors === 'object') {
			Object.keys(error.data.errors).forEach(field => {
				const fieldErrors = error.data.errors[field];
				validationErrors[field.toLowerCase()] = Array.isArray(fieldErrors) 
					? fieldErrors.join(', ') 
					: fieldErrors;
			});
		}
	}
	
	return validationErrors;
};

/**
 * Creates standardized query parameters for pagination
 * @param {number} page - Page number (0-based)
 * @param {number} pageSize - Number of items per page
 * @param {string} sortBy - Field to sort by
 * @param {string} sortOrder - Sort order ('asc' or 'desc')
 * @returns {object} Query parameters object
 */
export const createPaginationParams = (page = 0, pageSize = 10, sortBy = '', sortOrder = 'asc') => {
	const params = {
		page: page + 1, // Convert to 1-based for API
		pageSize,
	};
	
	if (sortBy) {
		params.sortBy = sortBy;
		params.sortOrder = sortOrder;
	}
	
	return params;
};

/**
 * Transforms API response to DataGrid-compatible format
 * @param {object} response - API response with data and pagination info
 * @returns {object} DataGrid-compatible response
 */
export const transformToDataGridResponse = (response) => {
	return {
		rows: response.data || [],
		rowCount: response.totalCount || 0,
		loading: false
	};
};
