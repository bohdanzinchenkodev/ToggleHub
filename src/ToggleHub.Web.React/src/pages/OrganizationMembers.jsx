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
	Chip,
	Select,
	MenuItem
} from '@mui/material';
import { DataGrid, GridActionsCellItem, GridRowModes, GridRowEditStopReasons } from '@mui/x-data-grid';
import {
	ArrowBack as ArrowBackIcon,
	Send as SendIcon,
	Edit as EditIcon,
	Save as SaveIcon,
	Cancel as CancelIcon
} from '@mui/icons-material';
import { Link } from 'react-router';
import {
	useSendOrganizationInviteMutation,
	useGetOrganizationInvitesQuery,
	useGetOrganizationMembersQuery,
	useResendOrganizationInviteMutation,
	useRevokeOrganizationInviteMutation,
	useUpdateOrganizationMemberRoleMutation
} from '../redux/slices/apiSlice';
import { useDispatch } from 'react-redux';
import {addNotification, showError, showSuccess} from '../redux/slices/notificationsSlice';
import { useAppState } from '../hooks/useAppState';
import AppStateDisplay from '../components/shared/AppStateDisplay';
import {
	PAGINATION_CONFIG,
	INVITE_STATUS,
	ORG_ROLES,
	MESSAGES
} from '../constants/organizationConstants';
import { getChipColor } from '../utils/organizationUtils';
import { validateEmail } from '../utils/validation';
import { formatDate } from '../utils/dateUtils';

const OrganizationMembers = () => {
	const [email, setEmail] = useState('');
	const [emailError, setEmailError] = useState('');
	const [invitesPaginationModel, setInvitesPaginationModel] = useState({
		page: PAGINATION_CONFIG.DEFAULT_PAGE,
		pageSize: PAGINATION_CONFIG.DEFAULT_PAGE_SIZE
	});
	const [membersPaginationModel, setMembersPaginationModel] = useState({
		page: PAGINATION_CONFIG.DEFAULT_PAGE,
		pageSize: PAGINATION_CONFIG.DEFAULT_PAGE_SIZE
	});
	const [rowModesModel, setRowModesModel] = useState({});
	const dispatch = useDispatch();

	const {
		currentOrganization: organization,
		orgSlug,
		isLoadingOrganization: isOrgLoading,
		isOrganizationError: isOrgError,
		organizationError: orgError
	} = useAppState();

	const handleEmailChange = (e) => {
		const newEmail = e.target.value;
		setEmail(newEmail);

		if (emailError) {
			setEmailError('');
		}
	};

	const [sendInvite, {
		isLoading: isSendingInvite,
		isError: isSendInviteError,
	}] = useSendOrganizationInviteMutation();

	const [revokeInviteMutation] = useRevokeOrganizationInviteMutation();
	const [resendInviteMutation] = useResendOrganizationInviteMutation();
	const [updateMemberRoleMutation] = useUpdateOrganizationMemberRoleMutation();

	// Custom hooks for invite actions
	const useInviteActions = () => {
		const revokeInvite = async (inviteId) => {
			try {
				await revokeInviteMutation({ organizationId: organization.id, inviteId }).unwrap();
				dispatch(showSuccess(MESSAGES.SUCCESS.INVITATION_REVOKED));
			} catch (err) {
				dispatch(showError(err.data?.detail || MESSAGES.ERROR.REVOKE_FAILED));
			}
		};

		const resendInvite = async (inviteId) => {
			try {
				await resendInviteMutation({ organizationId: organization.id, inviteId }).unwrap();
				dispatch(showSuccess(MESSAGES.SUCCESS.INVITATION_RESENT));
			} catch (err) {
				dispatch(showError(err.data?.detail || MESSAGES.ERROR.RESEND_FAILED));
			}
		};

		return { revokeInvite, resendInvite };
	};

	const { revokeInvite, resendInvite } = useInviteActions();

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

	// Column definitions
	const createInvitesColumns = () => [
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
			renderCell: (params) => (
				<Chip
					label={params.value}
					color={getChipColor(params.value)}
					size="small"
				/>
			)
		},
		{
			field: 'createdAt',
			headerName: 'Sent',
			width: 150,
			valueFormatter: (value) => formatDate(value)
		},
		{
			field: 'expiresAt',
			headerName: 'Expires',
			width: 150,
			valueFormatter: (value) => formatDate(value)
		},
		{
			field: 'actions',
			headerName: 'Actions',
			width: 200,
			sortable: false,
			renderCell: (params) => {
				const { id: inviteId, status } = params.row;
				const isPending = status === INVITE_STATUS.PENDING;

				return (
					<Box sx={{ display: 'flex', gap: 1, alignItems: 'center', height: '100%', justifyContent: 'center' }}>
						<Button
							variant="outlined"
							size="small"
							onClick={() => resendInvite(inviteId)}
							disabled={!isPending}
						>
							Resend
						</Button>
						<Button
							variant="outlined"
							size="small"
							color="error"
							onClick={() => revokeInvite(inviteId)}
							disabled={!isPending}
						>
							Revoke
						</Button>
					</Box>
				);
			}
		}
	];

	const invitesColumns = createInvitesColumns();

	const createMembersColumns = () => [
		{
			field: 'userFirstName',
			headerName: 'First Name',
			flex: 1,
			minWidth: 150,
			editable: false,
			valueGetter: (value, row) => row.user?.firstName || ''
		},
		{
			field: 'userLastName',
			headerName: 'Last Name',
			flex: 1,
			minWidth: 150,
			editable: false,
			valueGetter: (value, row) => row.user?.lastName || ''
		},
		{
			field: 'userEmail',
			headerName: 'Email',
			flex: 1,
			minWidth: 200,
			editable: false,
			valueGetter: (value, row) => row.user?.email || ''
		},
		{
			field: 'orgRole',
			headerName: 'Role',
			width: 150,
			editable: true,
			type: 'singleSelect',
			valueOptions: [ORG_ROLES.ADMIN, ORG_ROLES.FLAG_MANAGER],
			renderCell: (params) => (
				<Chip
					label={params.value}
					color={getChipColor(params.value)}
					size="small"
				/>
			)
		},
		{
			field: 'actions',
			type: 'actions',
			headerName: 'Actions',
			width: 100,
			cellClassName: 'actions',
			getActions: ({ id }) => {
				const isInEditMode = rowModesModel[id]?.mode === GridRowModes.Edit;
				const row = membersData?.data?.find(r => r.id === id);
				const isOwner = row?.orgRole === ORG_ROLES.OWNER;

				if (isOwner) {
					return [];
				}

				if (isInEditMode) {
					return [
						<GridActionsCellItem
							icon={<SaveIcon />}
							label="Save"
							sx={{ color: 'primary.main' }}
							onClick={handleSaveClick(id)}
						/>,
						<GridActionsCellItem
							icon={<CancelIcon />}
							label="Cancel"
							onClick={handleCancelClick(id)}
							color="inherit"
						/>,
					];
				}

				return [
					<GridActionsCellItem
						icon={<EditIcon />}
						label="Edit"
						onClick={handleEditClick(id)}
						color="inherit"
					/>,
				];
			},
		}
	];

	const membersColumns = createMembersColumns();

	const processRowUpdate = async (newRow) => {
		const oldRow = membersData?.data?.find(row => row.id === newRow.id);

		// Only process if the role actually changed
		if (oldRow && oldRow.orgRole !== newRow.orgRole) {
			try {
				await updateMemberRoleMutation({
					organizationId: organization.id,
					userId: newRow.user.id,
					newRole: newRow.orgRole,
				}).unwrap();
				dispatch(showSuccess(MESSAGES.SUCCESS.ROLE_UPDATED(newRow.user.email, newRow.orgRole)));
				return newRow;
			} catch (err) {
				dispatch(showError(err.data?.detail || MESSAGES.ERROR.ROLE_UPDATE_FAILED));
				throw err; // This will revert the change in the grid
			}
		}
		return newRow;
	};
	const handleRowEditStop = (params, event) => {
		if (params.reason === GridRowEditStopReasons.rowFocusOut) {
			event.defaultMuiPrevented = true;
		}
	};

	const handleEditClick = (id) => () => {
		setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.Edit } });
	};

	const handleSaveClick = (id) => () => {
		setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.View } });
	};

	const handleCancelClick = (id) => () => {
		setRowModesModel({
			...rowModesModel,
			[id]: { mode: GridRowModes.View, ignoreModifications: true },
		});
	};

	const isCellEditable = (params) => {
		// Only allow editing if:
		// 1. The user is not an Owner
		// 2. The row is currently in edit mode (controlled by Edit button)
		const isOwner = params.row.orgRole === ORG_ROLES.OWNER;
		const isInEditMode = rowModesModel[params.id]?.mode === GridRowModes.Edit;

		return !isOwner && isInEditMode;
	};

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

			dispatch(showSuccess(MESSAGES.SUCCESS.INVITATION_SENT(email)));
			setEmail('');
			setEmailError('');
		} catch (error) {
			const isServerError = error?.status === 500;
			const errorMessage = isServerError
				? MESSAGES.ERROR.SERVER_ERROR
				: error?.data?.detail || MESSAGES.ERROR.INVITATION_FAILED;

			dispatch(showError(errorMessage));
		}
	};

	if (isOrgLoading) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
					<CircularProgress />
				</Box>
			</Container>
		);
	}

	if (isOrgError) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Failed to load organization: {orgError?.data?.detail || MESSAGES.ERROR.ORGANIZATION_NOT_FOUND}
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
						{MESSAGES.ERROR.MEMBERS_LOAD_FAILED}: {membersError?.data?.detail || 'Unknown error'}
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
							pageSizeOptions={PAGINATION_CONFIG.PAGE_SIZE_OPTIONS}
							disableRowSelectionOnClick
							disableColumnSorting
							disableColumnFilter
							editMode="row"
							rowModesModel={rowModesModel}
							onRowModesModelChange={setRowModesModel}
							onRowEditStop={handleRowEditStop}
							processRowUpdate={processRowUpdate}
							isCellEditable={isCellEditable}
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
						{MESSAGES.ERROR.INVITES_LOAD_FAILED}: {invitesError?.data?.detail || 'Unknown error'}
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
							pageSizeOptions={PAGINATION_CONFIG.PAGE_SIZE_OPTIONS}
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
