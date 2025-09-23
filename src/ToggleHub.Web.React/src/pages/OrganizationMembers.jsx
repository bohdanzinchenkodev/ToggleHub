import React, { useState } from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress,
	TextField,
	Chip
} from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import { useParams } from 'react-router';
import { ArrowBack as ArrowBackIcon, Send as SendIcon } from '@mui/icons-material';
import { Link } from 'react-router';
import {
	useGetOrganizationBySlugQuery,
	useSendOrganizationInviteMutation,
	useGetOrganizationInvitesQuery,
	useGetOrganizationMembersQuery
} from '../redux/slices/apiSlice';
import { useDispatch } from 'react-redux';
import {addNotification, showError, showSuccess} from '../redux/slices/notificationsSlice';
import AppStateDisplay from '../components/shared/AppStateDisplay';

const OrganizationMembers = () => {
	const { orgSlug } = useParams();
	const [email, setEmail] = useState('');
	const [emailError, setEmailError] = useState('');
	const [invitesPaginationModel, setInvitesPaginationModel] = useState({ page: 0, pageSize: 10 });
	const [membersPaginationModel, setMembersPaginationModel] = useState({ page: 0, pageSize: 10 });
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

	// Get organization invites
	const {
		data: invitesData,
		isLoading: isInvitesLoading,
		error: invitesError
	} = useGetOrganizationInvitesQuery({
		organizationId: organization?.id,
		page: invitesPaginationModel.page + 1, // DataGrid uses 0-based, API uses 1-based
		pageSize: invitesPaginationModel.pageSize
	}, {
		skip: !organization?.id
	});

	// Get organization members
	const {
		data: membersData,
		isLoading: isMembersLoading,
		error: membersError
	} = useGetOrganizationMembersQuery({
		organizationId: organization?.id,
		page: membersPaginationModel.page + 1, // DataGrid uses 0-based, API uses 1-based
		pageSize: membersPaginationModel.pageSize
	}, {
		skip: !organization?.id
	});

	// Define columns for invites grid
	const invitesColumns = [
		{
			field: 'email',
			headerName: 'Email',
			flex: 1,
			minWidth: 200
		},
		{
			field: 'status',
			headerName: 'Status',
			width: 120,
			renderCell: (params) => {
				const status = params.value;
				let color = 'default';
				if (status === 'Pending') color = 'warning';
				else if (status === 'Accepted') color = 'success';
				else if (status === 'Declined') color = 'error';
				else if (status === 'Expired') color = 'error';

				return <Chip label={status} color={color} size="small" />;
			}
		},
		{
			field: 'createdAt',
			headerName: 'Sent',
			width: 150,
			valueFormatter: (value) => new Date(value).toLocaleDateString()
		},
		{
			field: 'expiresAt',
			headerName: 'Expires',
			width: 150,
			valueFormatter: (value) => new Date(value).toLocaleDateString()
		}
	];

	// Define columns for members grid
	const membersColumns = [
		{
			field: 'userFirstName',
			headerName: 'First Name',
			flex: 1,
			minWidth: 150,
			valueGetter: (value, row) => row.user?.firstName || ''
		},
		{
			field: 'userLastName',
			headerName: 'Last Name',
			flex: 1,
			minWidth: 150,
			valueGetter: (value, row) => row.user?.lastName || ''
		},
		{
			field: 'userEmail',
			headerName: 'Email',
			flex: 1,
			minWidth: 200,
			valueGetter: (value, row) => row.user?.email || ''
		},
		{
			field: 'orgRole',
			headerName: 'Role',
			width: 120,
			renderCell: (params) => {
				const role = params.value;
				let color = 'default';
				if (role === 'Owner') color = 'primary';
				else if (role === 'Admin') color = 'secondary';
				else if (role === 'Member') color = 'default';

				return <Chip label={role} color={color} size="small" />;
			}
		}
	];

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
			</Paper>

			{/* Members Grid */}
			<Paper sx={{ p: 4, mt: 3 }}>
				<Typography variant="h6" sx={{ mb: 2 }}>
					Members
				</Typography>
				{membersError ? (
					<Alert severity="error">
						Failed to load members: {membersError?.data?.detail || 'Unknown error'}
					</Alert>
				) : (
					<Box sx={{ width: '100%' }}>
						<DataGrid
							rows={membersData?.data || []}
							columns={membersColumns}
							loading={isMembersLoading}
							paginationModel={membersPaginationModel}
							onPaginationModelChange={setMembersPaginationModel}
							rowCount={membersData?.total || 0}
							paginationMode="server"
							pageSizeOptions={[5, 10, 25]}
							disableRowSelectionOnClick
							disableColumnSorting
							disableColumnFilter
						/>
					</Box>
				)}
			</Paper>

			{/* Invites Grid */}
			<Paper sx={{ p: 4, mt: 3 }}>
				<Typography variant="h6" sx={{ mb: 2 }}>
					Invites
				</Typography>
				{invitesError ? (
					<Alert severity="error">
						Failed to load invites: {invitesError?.data?.detail || 'Unknown error'}
					</Alert>
				) : (
					<Box sx={{ width: '100%' }}>
						<DataGrid
							rows={invitesData?.data || []}
							columns={invitesColumns}
							loading={isInvitesLoading}
							paginationModel={invitesPaginationModel}
							onPaginationModelChange={setInvitesPaginationModel}
							rowCount={invitesData?.total || 0}
							paginationMode="server"
							pageSizeOptions={[5, 10, 25]}
							disableRowSelectionOnClick
							disableColumnSorting
							disableColumnFilter
						/>
					</Box>
				)}
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
