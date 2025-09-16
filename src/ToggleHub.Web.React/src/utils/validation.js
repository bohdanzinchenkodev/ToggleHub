export const validateRequired = (value, fieldName) => {
	if (!value || !value.trim()) {
		return `${fieldName} is required`;
	}
	return null;
};

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
	});
	
	return errors;
};
