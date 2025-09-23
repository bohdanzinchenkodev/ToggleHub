import { useState } from 'react';

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

	const validateEmail = (email) => {
		if (!email) {
			return "Email is required";
		} else if (!/\S+@\S+\.\S+/.test(email)) {
			return "Email is invalid";
		}
		return null;
	};

	const validatePassword = (password, minLength = 6) => {
		if (!password) {
			return "Password is required";
		} else if (password.length < minLength) {
			return `Password must be at least ${minLength} characters`;
		}
		return null;
	};

	const validateRequired = (value, fieldName) => {
		if (!value) {
			return `${fieldName} is required`;
		}
		return null;
	};

	const validatePasswordMatch = (password, confirmPassword) => {
		if (!confirmPassword) {
			return "Please confirm your password";
		} else if (password !== confirmPassword) {
			return "Passwords do not match";
		}
		return null;
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
