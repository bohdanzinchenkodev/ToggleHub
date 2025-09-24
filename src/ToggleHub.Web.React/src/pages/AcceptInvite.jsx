import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router';
import {
	Container,
	Paper,
	Typography,
	Box,
	CircularProgress,
	Alert,
	Button
} from '@mui/material';
import { useAcceptOrganizationInviteMutation, useGetUserQuery } from '../redux/slices/apiSlice';

const AcceptInvite = () => {
	const { organizationId, token } = useParams();
	const navigate = useNavigate();
	const [errorMessage, setErrorMessage] = useState('');
	const [successMessage, setSuccessMessage] = useState('');
	const [isProcessed, setIsProcessed] = useState(false);

	const { data: user, isLoading: isUserLoading, error: userError } = useGetUserQuery();

	const [acceptInvite, { isLoading: isAccepting }] = useAcceptOrganizationInviteMutation();

	// Check if user is authenticated
	useEffect(() => {
		if (!isUserLoading && (!user || userError)) {
			// Redirect to login page with return URL
			navigate(`/login?returnUrl=${encodeURIComponent(window.location.pathname)}`);
		}
	}, [user, userError, isUserLoading, navigate]);

	// Automatically accept when user is authenticated
	useEffect(() => {
		const handleAccept = async () => {
			if (!user?.id || isProcessed) return;

			setIsProcessed(true);

			try {
				await acceptInvite({
					organizationId: parseInt(organizationId),
					body: {
						token,
						userId: user.id
					}
				}).unwrap();

				setSuccessMessage('Organization invitation accepted successfully');
			} catch (error) {
				// Don't show actual error details for server errors (500)
				const isServerError = error?.status === 500;
				const message = isServerError
					? 'An internal server error occurred. Please try again later.'
					: error?.data?.detail || 'Failed to accept invitation';

				setErrorMessage(message);
			}
		};

		if (user && organizationId && token) {
			handleAccept();
		}
	}, [user, organizationId, token, acceptInvite, isProcessed]);

	// Show loading while checking authentication
	if (isUserLoading || (!user && !userError)) {
		return (
			<Container maxWidth="sm" sx={{ py: 8 }}>
				<Paper sx={{ p: 4, textAlign: 'center' }}>
					<CircularProgress />
					<Typography sx={{ mt: 2 }}>
						Checking authentication...
					</Typography>
				</Paper>
			</Container>
		);
	}

	if (successMessage) {
		return (
			<Container maxWidth="sm" sx={{ py: 8 }}>
				<Paper sx={{ p: 4, textAlign: 'center' }}>
					<Alert severity="success" sx={{ mb: 3 }}>
						{successMessage}
					</Alert>
					<Button
						variant="contained"
						onClick={() => navigate('/')}
						size="large"
					>
						Go to Home
					</Button>
				</Paper>
			</Container>
		);
	}

	if (isAccepting) {
		return (
			<Container maxWidth="sm" sx={{ py: 8 }}>
				<Paper sx={{ p: 4, textAlign: 'center' }}>
					<Box sx={{ mb: 3 }}>
						<CircularProgress />
					</Box>
					<Typography variant="h5" gutterBottom>
						Accepting Invitation
					</Typography>
					<Typography color="text.secondary">
						Welcome, {user?.firstName || user?.email || 'User'}!
					</Typography>
					<Typography color="text.secondary">
						Please wait while we process your request...
					</Typography>
				</Paper>
			</Container>
		);
	}

	return (
		<Container maxWidth="sm" sx={{ py: 8 }}>
			<Paper sx={{ p: 4, textAlign: 'center' }}>
				<Typography variant="h4" gutterBottom>
					Accept Organization Invitation
				</Typography>

				<Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
					Welcome, {user?.firstName || user?.email || 'User'}!
				</Typography>

				{errorMessage && (
					<Alert severity="error" sx={{ mb: 3 }}>
						{errorMessage}
					</Alert>
				)}

				{errorMessage ? (
					<Button
						variant="contained"
						onClick={() => navigate('/')}
						size="large"
					>
						Go to Home
					</Button>
				) : (
					<Typography variant="body1" color="text.secondary">
						Processing your request...
					</Typography>
				)}
			</Paper>
		</Container>
	);
};

export default AcceptInvite;
