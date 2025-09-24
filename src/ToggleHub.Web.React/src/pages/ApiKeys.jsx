import React, { useState } from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress,
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Chip,
	IconButton,
	Tooltip
} from '@mui/material';
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
			environmentId: environment?.id
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
				) : apiKeysData && apiKeysData.data && apiKeysData.data.length > 0 ? (
					<TableContainer component={Paper} variant="outlined">
						<Table>
							<TableHead>
								<TableRow>
									<TableCell>API Key</TableCell>
									<TableCell>Status</TableCell>
									<TableCell>Environment</TableCell>
									<TableCell align="center">Actions</TableCell>
								</TableRow>
							</TableHead>
							<TableBody>
								{apiKeysData.data.map((apiKey, index) => (
									<TableRow key={index} hover>
										<TableCell>
											<Box sx={{ fontFamily: 'monospace', fontSize: '0.875rem' }}>
												{maskApiKey(apiKey.key)}
											</Box>
										</TableCell>
										<TableCell>
											<Chip
												label={apiKey.isActive ? 'Active' : 'Inactive'}
												color={apiKey.isActive ? 'success' : 'default'}
												size="small"
											/>
										</TableCell>
										<TableCell>
											{envType}
										</TableCell>
										<TableCell align="center">
											<Tooltip title="Copy API key to clipboard">
												<IconButton
													size="small"
													onClick={() => handleCopyApiKey(apiKey.key)}
													color="primary"
												>
													<CopyIcon fontSize="small" />
												</IconButton>
											</Tooltip>
										</TableCell>
									</TableRow>
								))}
							</TableBody>
						</Table>

						{apiKeysData.total > 0 && (
							<Box sx={{ p: 2, borderTop: 1, borderColor: 'divider' }}>
								<Typography variant="caption" color="text.secondary">
									Showing {apiKeysData.data.length} of {apiKeysData.total} API keys
								</Typography>
							</Box>
						)}
					</TableContainer>
				) : (
					<Alert severity="info">
						No API keys found for this environment.
					</Alert>
				)}
			</Paper>
		</Container>
	);
};

export default ApiKeys;
