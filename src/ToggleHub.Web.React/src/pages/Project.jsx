import React, { useEffect, useState } from 'react';
import {
	Box,
	Typography,
	CircularProgress,
	Alert,
	Container,
	Paper
} from "@mui/material";
import { Button } from '@mui/material';
import ConfirmDialog from '../components/shared/ConfirmDialog.jsx';
import { useDeleteProjectMutation } from '../redux/slices/apiSlice.js';
import { useNavigate } from 'react-router';
import { useDispatch } from 'react-redux';
import { showSuccess, showError } from '../redux/slices/notificationsSlice';
import {
	useGetFlagsByEnvironmentQuery
} from "../redux/slices/apiSlice.js";
import { useAppState } from "../hooks/useAppState.js";
import { useFlagOperations } from "../hooks/useFlagOperations.js";
import useInfiniteScrollQuery from "../hooks/useInfiniteScrollQuery.js";
import AppStateDisplay from "../components/shared/AppStateDisplay.jsx";
import EnvironmentTabs from "../components/project/EnvironmentTabs.jsx";
import EnvironmentContent from "../components/project/EnvironmentContent.jsx";
import { usePermissions } from '../hooks/usePermissions.js';
import { PERMISSIONS } from '../constants/permissions.js';

const Project = () => {
	const { hasPermission } = usePermissions();
	const [selectedTab, setSelectedTab] = useState(0);

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

	const handleTabChange = (event, newValue) => {
		setSelectedTab(newValue);
	};

	const selectedEnvironment = project?.environments?.[selectedTab];
	const {
		allItems: flags,
		totalCount,
		isLoading: isFlagsLoading,
		isError: isFlagsError,
		error: flagsError,
		hasNextPage,
		isFetchingNextPage,
		loadingRef
	} = useInfiniteScrollQuery({
		useQuery: useGetFlagsByEnvironmentQuery,
		baseQueryParams: {
			organizationId: organization?.id,
			projectId: project?.id,
			environmentId: selectedEnvironment?.id,
			pageSize: 10
		},
		skipCondition: !organization?.id || !project?.id || !selectedEnvironment?.id
	});

	const { localFlags, processingFlags, handleFlagToggle, syncFlags } = useFlagOperations(
		organization,
		project,
		selectedEnvironment
	);

	useEffect(() => {
		syncFlags(flags);
	}, [flags, syncFlags]);

	const canViewProject = hasPermission(PERMISSIONS.VIEW_PROJECTS) || hasPermission(PERMISSIONS.MANAGE_PROJECTS);

	const [deleteProject] = useDeleteProjectMutation();
	const [confirmOpen, setConfirmOpen] = React.useState(false);
	const navigate = useNavigate();
	const dispatch = useDispatch();

	const onDeleteClick = () => setConfirmOpen(true);
	const onDeleteCancel = () => setConfirmOpen(false);
	const onDeleteConfirm = async () => {
		setConfirmOpen(false);
		try {
			await deleteProject({ organizationId: organization.id, projectId: project.id }).unwrap();
			dispatch(showSuccess('Project deleted'));
			navigate(`/organizations/${orgSlug}`);
		} catch (err) {
			dispatch(showError(err?.data?.detail || 'Failed to delete project'));
		}
	};

	if (!canViewProject) {
		return (
			<Container component="main" maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
				<AppStateDisplay />
				<Paper sx={{ p: 3, mt: 4, textAlign: 'center' }}>
					<Typography variant="h6">Access denied</Typography>
					<Typography variant="body2" color="text.secondary">You don't have permission to access this project.</Typography>
				</Paper>
			</Container>
		);
	}

	if (isOrgLoading || isProjectLoading) {
		return (
			<Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
				<CircularProgress />
			</Box>
		);
	}

	if (isOrgError) {
		return (
			<Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
				<Alert severity="error">
					Failed to load organization: {orgError?.data?.detail || 'Organization not found'}
				</Alert>
			</Container>
		);
	}

	if (isProjectError) {
		return (
			<Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
				<Alert severity="error">
					Failed to load project: {projectError?.data?.detail || 'Project not found'}
				</Alert>
			</Container>
		);
	}

	return (
		<Container component="main" maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
			<AppStateDisplay />
			<Box>
				<Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
					<Typography variant="h4" component="h1" sx={{ fontWeight: 'bold' }}>
						{project ? project.name : `Project: ${projectSlug}`}
					</Typography>
					{hasPermission(PERMISSIONS.MANAGE_PROJECTS) && (
						<Button color="error" variant="outlined" onClick={onDeleteClick}>Delete Project</Button>
					)}
				</Box>

				{project?.environments && project.environments.length > 0 && (
					<Box sx={{
						display: 'flex',
						flexDirection: { xs: 'column', md: 'row' },
						mt: 4,
						minHeight: 400
					}}>
						<EnvironmentTabs
							environments={project.environments}
							selectedTab={selectedTab}
							onTabChange={handleTabChange}
						/>
						<EnvironmentContent
							environment={selectedEnvironment}
							flags={localFlags}
							totalCount={totalCount}
							isFlagsLoading={isFlagsLoading}
							isFlagsError={isFlagsError}
							flagsError={flagsError}
							processingFlags={processingFlags}
							onFlagToggle={handleFlagToggle}
							orgSlug={orgSlug}
							projectSlug={projectSlug}
							hasNextPage={hasNextPage}
							isFetchingNextPage={isFetchingNextPage}
							loadingRef={loadingRef}
						/>
					</Box>
				)}

				{!project && isProjectLoading && (
					<Typography variant="body1" sx={{ textAlign: 'center', color: 'text.secondary' }}>
						Loading project information...
					</Typography>
				)}

				{project && (!project.environments || project.environments.length === 0) && (
					<Paper sx={{ p: 3, mt: 4, textAlign: 'center' }}>
						<Typography variant="h6" sx={{ mb: 2 }}>No Environments</Typography>
						<Typography variant="body1" color="text.secondary">
							This project doesn't have any environments configured yet.
						</Typography>
					</Paper>
				)}
			</Box>
			<ConfirmDialog
				open={confirmOpen}
				title="Delete project?"
				content="This will permanently delete the project and all associated data. Are you sure?"
				onCancel={onDeleteCancel}
				onConfirm={onDeleteConfirm}
				confirmText="Delete"
				cancelText="Cancel"
				confirmColor="error"
			/>
		</Container>
	);
};

export default Project;
