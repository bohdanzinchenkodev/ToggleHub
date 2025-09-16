import { useState } from 'react';

export const useFormHandler = (initialData = {}) => {
	const [formData, setFormData] = useState(initialData);
	const [formErrors, setFormErrors] = useState({});

	const handleInputChange = (e) => {
		const { name, value } = e.target;
		setFormData(prev => ({
			...prev,
			[name]: value
		}));
		// Clear error when user starts typing
		if (formErrors[name]) {
			setFormErrors(prev => ({
				...prev,
				[name]: ""
			}));
		}
	};

	const handleServerErrors = (error) => {
		if (error?.status === 400 && error?.data?.errors) {
			const serverErrors = {};
			Object.keys(error.data.errors).forEach(field => {
				const errorMessages = error.data.errors[field];
				if (Array.isArray(errorMessages) && errorMessages.length > 0) {
					serverErrors[field] = errorMessages[0]; // Take the first error message
				}
			});
			setFormErrors(serverErrors);
		}
	};

	const resetForm = () => {
		setFormData(initialData);
		setFormErrors({});
	};

	const setErrors = (errors) => {
		setFormErrors(errors);
	};

	return {
		formData,
		formErrors,
		handleInputChange,
		handleServerErrors,
		resetForm,
		setErrors,
		setFormData
	};
};
