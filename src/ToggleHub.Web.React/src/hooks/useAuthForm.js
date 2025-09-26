import { useFormHandler } from './useFormHandler';
import { useFormValidation } from './useFormValidation';

export const useAuthForm = (initialFormData) => {
	const { formData, formErrors, handleInputChange, handleServerErrors, resetForm, setErrors, setFormData } = useFormHandler(initialFormData);
	const { 
		validateEmail, 
		validatePassword, 
		validateRequired, 
		validatePasswordMatch 
	} = useFormValidation();

	const validateLoginForm = () => {
		const newErrors = {};

		const emailError = validateEmail(formData.email);
		if (emailError) newErrors.email = emailError;

		const passwordError = validatePassword(formData.password);
		if (passwordError) newErrors.password = passwordError;

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	const validateRegisterForm = () => {
		const newErrors = {};

		const firstNameError = validateRequired(formData.firstName, "First name");
		if (firstNameError) newErrors.firstName = firstNameError;

		const lastNameError = validateRequired(formData.lastName, "Last name");
		if (lastNameError) newErrors.lastName = lastNameError;

		const emailError = validateEmail(formData.email);
		if (emailError) newErrors.email = emailError;

		const passwordError = validatePassword(formData.password);
		if (passwordError) newErrors.password = passwordError;

		const confirmPasswordError = validatePasswordMatch(formData.password, formData.confirmPassword);
		if (confirmPasswordError) newErrors.confirmPassword = confirmPasswordError;

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	const validateForgotPasswordForm = () => {
		const newErrors = {};

		const emailError = validateEmail(formData.email);
		if (emailError) newErrors.email = emailError;

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	const validateResetPasswordForm = () => {
		const newErrors = {};

		const emailError = validateEmail(formData.email);
		if (emailError) newErrors.email = emailError;

		const tokenError = validateRequired(formData.token, "Reset token");
		if (tokenError) newErrors.token = tokenError;

		const passwordError = validatePassword(formData.newPassword);
		if (passwordError) newErrors.newPassword = passwordError;

		const confirmPasswordError = validatePasswordMatch(formData.newPassword, formData.confirmPassword);
		if (confirmPasswordError) newErrors.confirmPassword = confirmPasswordError;

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	return {
		formData,
		errors: formErrors,
		handleChange: handleInputChange,
		handleServerErrors,
		resetForm,
		setFormData,
		setErrors,
		validateLoginForm,
		validateRegisterForm,
		validateForgotPasswordForm,
		validateResetPasswordForm
	};
};
