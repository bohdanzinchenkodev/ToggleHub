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
import { useDeclineOrganizationInviteMutation } from '../redux/slices/apiSlice';
import { CheckCircle as CheckCircleIcon, Cancel as CancelIcon } from '@mui/icons-material';

const DeclineInvite = () => {
	const { organizationId, token } = useParams();
	const navigate = useNavigate();
	const [errorMessage, setErrorMessage] = useState(null);

	const [declineInvite, {
		isLoading: isDeclining,
		isSuccess: isDeclined,
		isError: isDeclineError,
		error: declineError
	}] = useDeclineOrganizationInviteMutation();

	// Automatically decline when component mounts
	useEffect(() => {
		const handleDecline = async () => {
			try {
				await declineInvite({
					organizationId: parseInt(organizationId),
					token
				}).unwrap();

				// Success - no automatic redirect, just show success state
			} catch (error) {
				// Don't show actual error details for server errors (500)
				const isServerError = error?.status === 500;
				const message = isServerError
					? 'An internal server error occurred. Please try again later.'
					: error?.data?.detail || 'Failed to decline invitation';

				setErrorMessage(message);
			}
		};

		// Only call if we have the required params and haven't already started the process
		if (organizationId && token && !isDeclining && !isDeclined && !isDeclineError) {
			handleDecline();
		}
	}, [organizationId, token, isDeclining, isDeclined, isDeclineError, declineInvite, navigate]);

	if (isDeclined) {
		return (
			<Container maxWidth="sm" sx={{ py: 8 }}>
				<Paper sx={{ p: 4, textAlign: 'center' }}>
					<Alert severity="success" sx={{ mb: 3 }}>
						Organization invitation declined successfully
					</Alert>
					<Button
						variant="contained"
						onClick={() => navigate('/')}
						sx={{ mt: 2 }}
					>
						Go to Home
					</Button>
				</Paper>
			</Container>
		);
	}

	if (isDeclining) {
		return (
			<Container maxWidth="sm" sx={{ py: 8 }}>
				<Paper sx={{ p: 4, textAlign: 'center' }}>
					<Box sx={{ mb: 3 }}>
						<CircularProgress />
					</Box>
					<Typography variant="h5" gutterBottom>
						Declining Invitation
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
					Decline Organization Invitation
				</Typography>

				{errorMessage && (
					<>
						<Alert severity="error" sx={{ mb: 3 }}>
							{errorMessage}
						</Alert>
						<Button
							variant="contained"
							onClick={() => navigate('/')}
							sx={{ mt: 2 }}
						>
							Go to Home
						</Button>
					</>
				)}

				{!errorMessage && (
					<Typography variant="body1" color="text.secondary">
						Processing your request...
					</Typography>
				)}
			</Paper>
		</Container>
	);
};

export default DeclineInvite;
