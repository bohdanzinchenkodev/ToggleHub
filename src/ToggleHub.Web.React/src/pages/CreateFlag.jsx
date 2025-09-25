import React from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress
} from '@mui/material';
import { useParams } from 'react-router';
import { useDispatch } from 'react-redux';
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import {
	useCreateFlagMutation
} from '../redux/slices/apiSlice';
import { showSuccess, showError } from '../redux/slices/notificationsSlice';
import { useAppState } from '../hooks/useAppState';
import { useFlagForm } from '../hooks/useFlagForm';
import FlagForm from '../components/flag/FlagForm';
import { Link, useNavigate } from 'react-router';
import { usePermissions } from '../hooks/usePermissions';
import { PERMISSIONS } from '../constants/permissions';

const CreateFlag = () => {
	const { hasPermission } = usePermissions();
	const { envType } = useParams();
	const navigate = useNavigate();
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

	// Find the environment by type
	const environment = project?.environments?.find(env => env.type === envType);

	// Create flag mutation
	const [createFlag, {
		isLoading: isCreating,
		error: createError,
		isError: isCreateError
	}] = useCreateFlagMutation();

	const {
		formData,
		formErrors,
		handleInputChange,
		handleReturnTypeChange,
		handleServerErrors,
		validateForm,
		prepareFlagData,
		ruleSetManager
	} = useFlagForm();

	const handleSubmit = async (event) => {
		event.preventDefault();

		if (!validateForm()) {
			return;
		}

		if (!organization?.id || !project?.id || !environment?.id) {
			console.error('Missing required IDs:', {
				organization: organization?.id,
				project: project?.id,
				environment: environment?.id
			});
			return;
		}

		try {
			const flagData = prepareFlagData();

			const result = await createFlag({
				organizationId: organization.id,
				projectId: project.id,
				environmentId: environment.id,
				body: flagData
			}).unwrap();

			// Show success notification and redirect to edit page
			dispatch(showSuccess(`Flag "${result.key}" created successfully!`));
			navigate(`/organizations/${orgSlug}/projects/${projectSlug}/environments/${envType}/flags/${result.id}/edit`);
		} catch (error) {
			handleServerErrors(error);
			dispatch(showError('Failed to create flag. Please check the form for errors.'));
			console.error('Error creating flag:', error);
		}
	};

	const handleGoBack = () => {
		navigate(`/organizations/${orgSlug}/projects/${projectSlug}`);
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

	const canManageFlags = hasPermission(PERMISSIONS.MANAGE_FLAGS);

	if (!canManageFlags) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Paper sx={{ p: 3, textAlign: 'center' }}>
					<Typography variant="h6">Access denied</Typography>
					<Typography variant="body2" color="text.secondary">You don't have permission to create flags in this environment.</Typography>
				</Paper>
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

				<Typography variant="h5" sx={{ mb: 3 }}>
					Create New Flag
				</Typography>

				<Box sx={{ mb: 3, p: 2, borderRadius: 1 }}>
					<Typography variant="body2" color="text.secondary">
						<strong>Environment:</strong> {envType} • <strong>Project:</strong> {projectSlug} • <strong>Organization:</strong> {orgSlug}
					</Typography>
				</Box>

				<FlagForm
					mode="create"
					formData={formData}
					formErrors={formErrors}
					isSubmitting={isCreating}
					submitError={isCreateError ? createError : null}
					onInputChange={handleInputChange}
					onReturnTypeChange={handleReturnTypeChange}
					onSubmit={handleSubmit}
					onCancel={handleGoBack}
					ruleSetManager={ruleSetManager}
				/>
			</Paper>
		</Container>
	);
};
export default CreateFlag;
