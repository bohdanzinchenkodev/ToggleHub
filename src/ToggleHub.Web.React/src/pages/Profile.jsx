import React, { useEffect } from 'react';
import {
	Container,
	Paper,
	Typography,
	TextField,
	Button,
	Box,
	Alert,
	CircularProgress,
	Avatar,
	Divider
} from '@mui/material';
import {
	Save as SaveIcon,
	Person as PersonIcon
} from '@mui/icons-material';
import { useGetUserQuery, useUpdateUserMutation } from '../redux/slices/apiSlice.js';
import { useFormHandler } from '../hooks/useFormHandler.js';
import { validateForm } from '../utils/validation.js';
import { getUserInitials } from '../utils/userUtils.js';

const Profile = () => {
	const { data: user, isLoading: isLoadingUser } = useGetUserQuery();
	const [updateUser, { isLoading: isUpdating, error: updateError, isSuccess }] = useUpdateUserMutation();

	const {
		formData,
		formErrors,
		handleInputChange,
		handleServerErrors,
		setErrors,
		setFormData
	} = useFormHandler({
		email: '',
		firstName: '',
		lastName: ''
	});

	// Populate form when user data is loaded
	useEffect(() => {
		if (user) {
			setFormData({
				email: user.email || '',
				firstName: user.firstName || '',
				lastName: user.lastName || ''
			});
		}
	}, [user, setFormData]);

	const handleSubmit = async (e) => {
		e.preventDefault();

		// Validation
		const validationErrors = validateForm(formData, {
			email: { required: true, email: true, label: "Email" },
			firstName: { required: true, label: "First name" },
			lastName: { required: true, label: "Last name" }
		});

		if (Object.keys(validationErrors).length > 0) {
			setErrors(validationErrors);
			return;
		}

		try {
			await updateUser({
				email: formData.email.trim(),
				firstName: formData.firstName.trim(),
				lastName: formData.lastName.trim()
			}).unwrap();
		} catch (error) {
			console.error('Failed to update profile:', error);
			handleServerErrors(error);
		}
	};

	if (isLoadingUser) {
		return (
			<Container maxWidth="md" sx={{ mt: 4, display: 'flex', justifyContent: 'center' }}>
				<CircularProgress />
			</Container>
		);
	}

	return (
		<Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
			<Paper elevation={3} sx={{ p: 4 }}>
				<Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
					<Avatar
						sx={{
							width: 64,
							height: 64,
							bgcolor: 'primary.main',
							fontSize: '1.5rem',
							mr: 3
						}}
					>
						{getUserInitials(user)}
					</Avatar>
					<Box>
						<Typography variant="h4" component="h1" gutterBottom>
							Profile Settings
						</Typography>
						<Typography variant="body1" color="text.secondary">
							Update your personal information
						</Typography>
					</Box>
				</Box>

				<Divider sx={{ mb: 3 }} />

				{isSuccess && (
					<Alert severity="success" sx={{ mb: 3 }}>
						Profile updated successfully!
					</Alert>
				)}

				{updateError && (
					<Alert severity="error" sx={{ mb: 3 }}>
						{updateError.data?.detail || 'Failed to update profile. Please try again.'}
					</Alert>
				)}

				<Box component="form" onSubmit={handleSubmit}>
					<TextField
						fullWidth
						label="Email Address"
						name="email"
						type="email"
						value={formData.email}
						onChange={handleInputChange}
						error={!!formErrors.email}
						helperText={formErrors.email}
						margin="normal"
						required
					/>

					<TextField
						fullWidth
						label="First Name"
						name="firstName"
						value={formData.firstName}
						onChange={handleInputChange}
						error={!!formErrors.firstName}
						helperText={formErrors.firstName}
						margin="normal"
						required
					/>

					<TextField
						fullWidth
						label="Last Name"
						name="lastName"
						value={formData.lastName}
						onChange={handleInputChange}
						error={!!formErrors.lastName}
						helperText={formErrors.lastName}
						margin="normal"
						required
					/>

					<Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end' }}>
						<Button
							type="submit"
							variant="contained"
							startIcon={isUpdating ? <CircularProgress size={20} /> : <SaveIcon />}
							disabled={isUpdating}
							size="large"
						>
							{isUpdating ? 'Saving...' : 'Save Changes'}
						</Button>
					</Box>
				</Box>
			</Paper>
		</Container>
	);
};

export default Profile;
