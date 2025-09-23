import React, { useState } from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress,
	TextField
} from '@mui/material';
import { useParams } from 'react-router';
import { ArrowBack as ArrowBackIcon, Send as SendIcon } from '@mui/icons-material';
import { Link } from 'react-router';
import {
	useGetOrganizationBySlugQuery,
	useSendOrganizationInviteMutation
} from '../redux/slices/apiSlice';
import { useDispatch } from 'react-redux';
import {addNotification, showError, showSuccess} from '../redux/slices/notificationsSlice';
import AppStateDisplay from '../components/shared/AppStateDisplay';

const OrganizationMembers = () => {
	const { orgSlug } = useParams();
	const [email, setEmail] = useState('');
	const [emailError, setEmailError] = useState('');
	const dispatch = useDispatch();

	// Email validation function
	const validateEmail = (email) => {
		const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
		if (!email.trim()) {
			return 'Email is required';
		}
		if (!emailRegex.test(email.trim())) {
			return 'Please enter a valid email address';
		}
		return '';
	};

	const handleEmailChange = (e) => {
		const newEmail = e.target.value;
		setEmail(newEmail);

		// Clear error when user starts typing
		if (emailError) {
			setEmailError('');
		}
	};

	const {
		data: organization,
		isLoading: isOrgLoading,
		isError: isOrgError,
		error: orgError
	} = useGetOrganizationBySlugQuery(orgSlug);

	const [sendInvite, {
		isLoading: isSendingInvite,
		isError: isSendInviteError,
	}] = useSendOrganizationInviteMutation();

	const handleSendInvite = async () => {
		// Validate email before sending
		const validationError = validateEmail(email);
		if (validationError) {
			setEmailError(validationError);
			return;
		}

		if (!organization?.id) return;

		try {
			await sendInvite({
				organizationId: organization.id,
				body: {
					email: email.trim(),
					organizationId: organization.id
				}
			}).unwrap();

			dispatch(showSuccess( `Invitation sent to ${email}`));

			setEmail('');
			setEmailError('');
		} catch (error) {
			// Don't show actual error details for server errors (500)
			const isServerError = error?.status === 500;
			const errorMessage = isServerError 
				? 'An internal server error occurred. Please try again later.'
				: error?.data?.detail || 'Failed to send invitation';
			
			dispatch(showError(errorMessage));
		}
	};

	// Loading state
	if (isOrgLoading) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
					<CircularProgress />
				</Box>
			</Container>
		);
	}

	// Error state
	if (isOrgError) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Failed to load organization: {orgError?.data?.detail || 'Organization not found'}
				</Alert>
			</Container>
		);
	}

	return (
		<Container maxWidth="lg" sx={{ py: 3 }}>
			<Paper sx={{ p: 4 }}>
				<Box sx={{ mb: 3, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
					<Box sx={{ display: 'flex', alignItems: 'center', gap: 2, minHeight: '40px' }}>
						<Box sx={{
							'& > *': { mb: 0 },
							display: 'flex',
							alignItems: 'center'
						}}>
							<AppStateDisplay />
						</Box>
						<Button
							startIcon={<ArrowBackIcon />}
							component={Link}
							to={`/organizations/${orgSlug}`}
							variant="outlined"
						>
							Back to Organization
						</Button>
					</Box>
				</Box>

				<Typography variant="h5" sx={{ mb: 3 }}>
					Organization Members
				</Typography>

				<Box sx={{ mb: 3, p: 2, borderRadius: 1 }}>
					<Typography variant="body2" color="text.secondary">
						<strong>Organization:</strong> {organization?.name}
					</Typography>
				</Box>

				{/* TODO: Add members list/table here */}
				<Alert severity="info">
					Members management functionality will be implemented here.
					This will include:
					<ul>
						<li>List of current organization members</li>
						<li>Member roles and permissions</li>
						<li>Remove/manage existing members</li>
					</ul>
				</Alert>
			</Paper>

			{/* Invite Member Card */}
			<Paper sx={{ p: 4, mt: 3 }}>
				<Typography variant="h6" sx={{ mb: 2 }}>
					Invite Member
				</Typography>
				<Box sx={{ display: 'flex', gap: 2, alignItems: 'flex-start' }}>
					<TextField
						label="Email Address"
						type="email"
						value={email}
						onChange={handleEmailChange}
						onKeyPress={(e) => {
							if (e.key === 'Enter' && email.trim() && !isSendingInvite && !emailError) {
								handleSendInvite();
							}
						}}
						placeholder="Enter member's email"
						sx={{ flex: 1 }}
						size="small"
						disabled={isSendingInvite}
						error={!!emailError}
						helperText={emailError}
					/>
					<Button
						variant="contained"
						startIcon={<SendIcon />}
						onClick={handleSendInvite}
						disabled={!email.trim() || !!emailError || isSendingInvite}
						loading={isSendingInvite}
					>
						{isSendingInvite ? 'Sending...' : 'Send Invite'}
					</Button>
				</Box>
			</Paper>
		</Container>
	);
};

export default OrganizationMembers;
