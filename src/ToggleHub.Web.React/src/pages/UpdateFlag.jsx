import React, { useEffect } from 'react';
import {
	Box,
	Typography,
	Paper,
	Button,
	Alert,
	Container,
	CircularProgress
} from '@mui/material';
import ConfirmDialog from '../components/shared/ConfirmDialog';
import { useParams, Link, useNavigate } from 'react-router';
import { useDispatch } from 'react-redux';
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import {
	useGetFlagByIdQuery,
	useUpdateFlagMutation,
	useDeleteFlagMutation
} from '../redux/slices/apiSlice';
import { showSuccess, showError } from '../redux/slices/notificationsSlice';
import { useAppState } from '../hooks/useAppState';
import { useFlagForm } from '../hooks/useFlagForm';
import FlagForm from '../components/flag/FlagForm';
import { usePermissions } from '../hooks/usePermissions';
import { PERMISSIONS } from '../constants/permissions';

const UpdateFlag = () => {
	const { envType, flagId } = useParams();
	const dispatch = useDispatch();
	const navigate = useNavigate();

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

	const {
		data: flag,
		isLoading: isFlagLoading,
		isError: isFlagError,
		error: flagError
	} = useGetFlagByIdQuery(
		{
			organizationId: organization?.id,
			projectId: project?.id,
			environmentId: environment?.id,
			flagId: parseInt(flagId)
		},
		{ skip: !organization?.id || !project?.id || !environment?.id }
	);

	// Update flag mutation
	const [updateFlag, {
		isLoading: isUpdating,
		error: updateError,
		isError: isUpdateError
	}] = useUpdateFlagMutation();
	const { hasPermission } = usePermissions();

	const canManageFlags = hasPermission(PERMISSIONS.MANAGE_FLAGS);
	const [deleteFlag, { isLoading: isDeleting }] = useDeleteFlagMutation();

	const {
		formData,
		formErrors,
		handleInputChange,
		handleReturnTypeChange,
		handleServerErrors,
		validateForm,
		prepareFlagData,
		ruleSetManager,
		setFormData
	} = useFlagForm();

	// Initialize form with flag data when flag is loaded
	useEffect(() => {
		if (flag) {
			setFormData({
				key: flag.key || '',
				description: flag.description || '',
				enabled: flag.enabled || false,
				returnValueType: flag.returnValueType || 'Boolean',
				defaultValueOnRaw: flag.defaultValueOnRaw || '',
				defaultValueOffRaw: flag.defaultValueOffRaw || '',
				ruleSets: (flag.ruleSets || []).map(ruleSet => ({
					...ruleSet,
					conditions: (ruleSet.conditions || []).map(condition => ({
						...condition,
						// Ensure correct field names for frontend
						fieldType: condition.fieldType || condition.fieldTypeString || '',
						operator: condition.operator || condition.operatorString || '',
						items: condition.items || []
					}))
				}))
			});
		}
	}, [flag, setFormData]);

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

			const result = await updateFlag({
				organizationId: organization.id,
				projectId: project.id,
				environmentId: environment.id,
				flagId: parseInt(flagId),
				body: flagData
			}).unwrap();

			// Show success notification
			dispatch(showSuccess(`Flag "${result.key}" updated successfully!`));
		} catch (error) {
			handleServerErrors(error);
			dispatch(showError('Failed to update flag. Please check the form for errors.'));
			console.error('Error updating flag:', error);
		}
	};

	const handleGoBack = () => {
		navigate(`/organizations/${orgSlug}/projects/${projectSlug}`);
	};

	const [confirmOpen, setConfirmOpen] = React.useState(false);

	const onDeleteClick = () => setConfirmOpen(true);
	const onDeleteCancel = () => setConfirmOpen(false);

	const onDeleteConfirm = async () => {
		setConfirmOpen(false);
		if (!organization?.id || !project?.id || !environment?.id || !flagId) return;
		try {
			await deleteFlag({
				organizationId: organization.id,
				projectId: project.id,
				environmentId: environment.id,
				flagId: parseInt(flagId)
			}).unwrap();
			dispatch(showSuccess('Flag deleted successfully'));
			navigate(`/organizations/${orgSlug}/projects/${projectSlug}`);
		} catch (error) {
			dispatch(showError('Failed to delete flag'));
		}
	};

	if (isOrgLoading || isProjectLoading || isFlagLoading) {
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
					{orgError?.data?.detail || 'Failed to load organization'}
				</Alert>
			</Container>
		);
	}

	if (isProjectError) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					{projectError?.data?.detail || 'Failed to load project'}
				</Alert>
			</Container>
		);
	}

	if (isFlagError) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					{flagError?.data?.detail || 'Failed to load flag'}
				</Alert>
			</Container>
		);
	}

	if (!environment) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Environment "{envType}" not found in project "{projectSlug}"
				</Alert>
			</Container>
		);
	}

	if (!flag) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Alert severity="error">
					Flag not found
				</Alert>
			</Container>
		);
	}

	if (!canManageFlags) {
		return (
			<Container maxWidth="lg" sx={{ py: 3 }}>
				<Paper sx={{ p: 3, textAlign: 'center' }}>
					<Typography variant="h6">Access denied</Typography>
					<Typography variant="body2" color="text.secondary">You don't have permission to manage flags in this environment.</Typography>
				</Paper>
			</Container>
		);
	}

	return (
		<Container maxWidth="lg" sx={{ py: 3, position: 'relative' }}>
			<Paper sx={{ p: 4 }}>
				<Box sx={{ display: 'flex', alignItems: 'center', mb: 3, gap: 2, justifyContent: 'space-between' }}>
					<Box>
						<Button
							startIcon={<ArrowBackIcon />}
							component={Link}
							to={`/organizations/${orgSlug}/projects/${projectSlug}`}
							variant="outlined"
						>
							Back to Project
						</Button>
					</Box>
					<Box>
						{hasPermission(PERMISSIONS.MANAGE_FLAGS) && (
							<Button
								color="error"
								variant="outlined"
								disabled={isDeleting}
								onClick={onDeleteClick}
							>
								Delete Flag
							</Button>
						)}
					</Box>
				</Box>

				<Typography variant="h5" sx={{ mb: 3 }}>
					Update Flag: {flag.key}
				</Typography>

				<Box sx={{ mb: 3, p: 2, borderRadius: 1 }}>
					<Typography variant="body2" color="text.secondary">
						<strong>Environment:</strong> {envType} • <strong>Project:</strong> {projectSlug} • <strong>Organization:</strong> {orgSlug}
					</Typography>
				</Box>

				<FlagForm
					mode="update"
					formData={formData}
					formErrors={formErrors}
					isSubmitting={isUpdating}
					submitError={isUpdateError ? updateError : null}
					onInputChange={handleInputChange}
					onReturnTypeChange={handleReturnTypeChange}
					onSubmit={handleSubmit}
					onCancel={handleGoBack}
					ruleSetManager={ruleSetManager}
				/>

			</Paper>

			<ConfirmDialog
				open={confirmOpen}
				title="Delete flag?"
				content="This action will permanently delete the flag. Are you sure you want to continue?"
				onCancel={onDeleteCancel}
				onConfirm={onDeleteConfirm}
				confirmText="Delete"
				cancelText="Cancel"
				confirmColor="error"
			/>

		</Container>
	);
}

export default UpdateFlag;
