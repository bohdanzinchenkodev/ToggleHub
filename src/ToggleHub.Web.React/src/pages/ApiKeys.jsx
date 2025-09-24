import React, { useState } from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress,
	Chip,
	IconButton,
	Tooltip
} from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import { useAppState } from '../hooks/useAppState';
import { useParams } from 'react-router';
import { useDispatch } from 'react-redux';
import { ArrowBack as ArrowBackIcon, Key as KeyIcon, ContentCopy as CopyIcon } from '@mui/icons-material';
import {
	useGetApiKeysQuery
} from '../redux/slices/apiSlice';
import { showSuccess, showError } from '../redux/slices/notificationsSlice';
import { Link } from 'react-router';

const ApiKeys = () => {
	const dispatch = useDispatch();
	const [paginationModel, setPaginationModel] = useState({
		page: 0,
		pageSize: 25,
	});

	const {
		currentOrganization: organization,
		currentProject: project,
		orgSlug,
		projectSlug,
		isLoadingOrganization: isOrgLoading,
		isLoadingProject: isProjectLoading,
		isOrganizationError: isOrgError,
		isProjectError: isProjectError,
		organizationError: orgError,
		projectError: projectError
	} = useAppState();

	const { envType } = useParams();

	const environment = project?.environments?.find(env => env.type === envType);

	const {
		data: apiKeysData,
		isLoading: isApiKeysLoading,
		isError: isApiKeysError,
		error: apiKeysError
	} = useGetApiKeysQuery(
		{
			organizationId: organization?.id,
			projectId: project?.id,
			environmentId: environment?.id,
			page: paginationModel.page + 1, // DataGrid uses 0-based, API uses 1-based
			pageSize: paginationModel.pageSize
		},
		{
			skip: !organization?.id || !project?.id || !environment?.id
		}
	);

	const maskApiKey = (key) => {
		if (!key || key.length <= 8) return key;
		return `${key.substring(0, 8)}${'*'.repeat(key.length - 8)}`;
	};

	const handleCopyApiKey = async (apiKey) => {
		try {
			await navigator.clipboard.writeText(apiKey);
			dispatch(showSuccess('API key copied to clipboard'));
		} catch (error) {
			dispatch(showError('Failed to copy API key'));
			console.error('Failed to copy to clipboard:', error);
		}
	};

	const columns = [
		{
			field: 'key',
			headerName: 'API Key',
			flex: 1,
			renderCell: (params) => (
				<Box sx={{ fontFamily: 'monospace', fontSize: '0.875rem' }}>
					{maskApiKey(params.value)}
				</Box>
			),
		},
		{
			field: 'isActive',
			headerName: 'Status',
			width: 120,
			renderCell: (params) => (
				<Chip
					label={params.value ? 'Active' : 'Inactive'}
					color={params.value ? 'success' : 'default'}
					size="small"
				/>
			),
		},
		{
			field: 'environment',
			headerName: 'Environment',
			width: 150,
			valueGetter: () => envType,
		},
		{
			field: 'actions',
			headerName: 'Actions',
			width: 100,
			sortable: false,
			renderCell: (params) => (
				<Tooltip title="Copy API key to clipboard">
					<IconButton
						size="small"
						onClick={() => handleCopyApiKey(params.row.key)}
						color="primary"
					>
						<CopyIcon fontSize="small" />
					</IconButton>
				</Tooltip>
			),
		},
	];

	const rows = apiKeysData?.data?.map((apiKey, index) => ({
		id: index,
		...apiKey
	})) || [];

	if (isOrgLoading || isProjectLoading) {
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
					Failed to load organization: {orgError?.data?.detail || 'Organization not found'}
				</Alert>
			</Container>
		);
	}

	if (isProjectError) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Failed to load project: {projectError?.data?.detail || 'Project not found'}
				</Alert>
			</Container>
		);
	}

	if (!environment) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Environment "{envType}" not found in project
				</Alert>
			</Container>
		);
	}

	return (
		<Container maxWidth="lg" sx={{ py: 3 }}>
			<Paper sx={{ p: 4 }}>
				<Box sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
					<Button
						startIcon={<ArrowBackIcon />}
						component={Link}
						to={`/organizations/${orgSlug}/projects/${projectSlug}`}
						variant="outlined"
					>
						Back to Project
					</Button>
				</Box>

				<Typography variant="h5" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
					<KeyIcon />
					API Keys
				</Typography>

				<Box sx={{ mb: 3, p: 2, borderRadius: 1 }}>
					<Typography variant="body2" color="text.secondary">
						<strong>Environment:</strong> {envType} • <strong>Project:</strong> {projectSlug} • <strong>Organization:</strong> {orgSlug}
					</Typography>
				</Box>

				{isApiKeysLoading ? (
					<Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
						<CircularProgress />
					</Box>
				) : isApiKeysError ? (
					<Alert severity="error" sx={{ mb: 2 }}>
						Failed to load API keys: {apiKeysError?.data?.detail || 'Unknown error'}
					</Alert>
				) : (
					<Box sx={{ height: 600, width: '100%' }}>
						<DataGrid
							rows={rows}
							columns={columns}
							paginationMode="server"
							paginationModel={paginationModel}
							onPaginationModelChange={setPaginationModel}
							pageSizeOptions={[10, 25, 50, 100]}
							rowCount={apiKeysData?.total || 0}
							loading={isApiKeysLoading}
							disableRowSelectionOnClick
							disableColumnMenu
							disableColumnSorting
							sx={{
								border: 1,
								borderColor: 'divider',
								'& .MuiDataGrid-cell:focus': {
									outline: 'none',
								},
								'& .MuiDataGrid-row:hover': {
									backgroundColor: 'action.hover',
								},
							}}
							slots={{
								noRowsOverlay: () => (
									<Box
										sx={{
											display: 'flex',
											flexDirection: 'column',
											alignItems: 'center',
											justifyContent: 'center',
											height: '100%',
											gap: 2,
										}}
									>
										<KeyIcon sx={{ fontSize: 48, color: 'text.secondary' }} />
										<Typography variant="h6" color="text.secondary">
											No API keys found
										</Typography>
										<Typography variant="body2" color="text.secondary">
											No API keys found for this environment.
										</Typography>
									</Box>
								),
							}}
						/>
					</Box>
				)}
			</Paper>
		</Container>
	);
};

export default ApiKeys;
