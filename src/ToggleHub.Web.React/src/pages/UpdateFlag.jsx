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
import { useParams } from 'react-router';
import { useDispatch } from 'react-redux';
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import {
	useGetFlagByIdQuery,
	useUpdateFlagMutation
} from '../redux/slices/apiSlice';
import { showSuccess, showError } from '../redux/slices/notificationsSlice';
import { useAppState } from '../hooks/useAppState';
import { useFlagForm } from '../hooks/useFlagForm';
import FlagForm from '../components/flag/FlagForm';
import { Link } from 'react-router';

const UpdateFlag = () => {
	const { envType, flagId } = useParams();
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

	return (
		<Container maxWidth="lg" sx={{ py: 3 }}>
			<Paper sx={{ p: 4 }}>
				<Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
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
		</Container>
	);
};

export default UpdateFlag;
