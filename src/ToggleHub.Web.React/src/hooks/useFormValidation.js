import { useState } from 'react';
import { 
	validateEmail, 
	validatePassword, 
	validateRequired, 
	validatePasswordMatch 
} from '../utils/validation';

export const useFormValidation = () => {
	const [errors, setErrors] = useState({});

	const clearError = (fieldName) => {
		if (errors[fieldName]) {
			setErrors(prev => ({
				...prev,
				[fieldName]: ""
			}));
		}
	};

	const setError = (fieldName, message) => {
		setErrors(prev => ({
			...prev,
			[fieldName]: message
		}));
	};

	const setMultipleErrors = (newErrors) => {
		setErrors(newErrors);
	};

	return {
		errors,
		clearError,
		setError,
		setMultipleErrors,
		validateEmail,
		validatePassword,
		validateRequired,
		validatePasswordMatch
	};
};
