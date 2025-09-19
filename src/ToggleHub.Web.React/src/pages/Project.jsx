import React, { useEffect, useState } from 'react';
import {
	Box,
	Typography,
	CircularProgress,
	Alert,
	Container,
	Paper
} from "@mui/material";
import { useParams } from "react-router";
import {
	useGetOrganizationBySlugQuery,
	useGetProjectBySlugQuery,
	useGetFlagsByEnvironmentQuery
} from "../redux/slices/apiSlice.js";
import { useAppState } from "../hooks/useAppState.js";
import { useFlagOperations } from "../hooks/useFlagOperations.js";
import AppStateDisplay from "../components/shared/AppStateDisplay.jsx";
import EnvironmentTabs from "../components/project/EnvironmentTabs.jsx";
import EnvironmentContent from "../components/project/EnvironmentContent.jsx";

const Project = () => {
	const { orgSlug, projectSlug } = useParams();
	const { currentOrganization, currentProject, updateCurrentOrganization, updateCurrentProject } = useAppState();
	const [selectedTab, setSelectedTab] = useState(0);

	const handleTabChange = (event, newValue) => {
		setSelectedTab(newValue);
	};

	// Only fetch organization if we don't have it in state or if the slug doesn't match
	const shouldFetchOrg = !currentOrganization || currentOrganization.slug !== orgSlug;

	// Get organization details by slug (only if needed)
	const {
		data: fetchedOrganization,
		isLoading: isOrgLoading,
		isError: isOrgError,
		error: orgError
	} = useGetOrganizationBySlugQuery(orgSlug, {
		skip: !shouldFetchOrg
	});

	// Use organization from state if available, otherwise use fetched data
	const organization = currentOrganization?.slug === orgSlug ? currentOrganization : fetchedOrganization;

	// Only fetch project if we don't have it in state or if the slug doesn't match
	const shouldFetchProject = !currentProject || currentProject.slug !== projectSlug;

	const {
		data: fetchedProject,
		isLoading: isProjectLoading,
		isError: isProjectError,
		error: projectError
	} = useGetProjectBySlugQuery(
		{ orgSlug, projectSlug, organizationId: organization?.id },
		{
			skip: !shouldFetchProject || !organization?.id
		}
	);

	const project = currentProject?.slug === projectSlug ? currentProject : fetchedProject;

	// Get flags for the currently selected environment
	const selectedEnvironment = project?.environments?.[selectedTab];
	const {
		data: flags,
		isLoading: isFlagsLoading,
		isError: isFlagsError,
		error: flagsError
	} = useGetFlagsByEnvironmentQuery(
		{
			organizationId: organization?.id,
			projectId: project?.id,
			environmentId: selectedEnvironment?.id
		},
		{
			skip: !organization?.id || !project?.id || !selectedEnvironment?.id
		}
	);

	// Flag operations hook
	const { localFlags, processingFlags, handleFlagToggle, syncFlags } = useFlagOperations(
		organization, 
		project, 
		selectedEnvironment
	);

	// Sync API flags to local state
	useEffect(() => {
		syncFlags(flags);
	}, [flags, syncFlags]);

	// Update global state when organization is fetched (only if we didn't have it)
	useEffect(() => {
		if (fetchedOrganization && shouldFetchOrg) {
			updateCurrentOrganization(fetchedOrganization);
		}
	}, [fetchedOrganization, shouldFetchOrg, updateCurrentOrganization]);

	// Update global state when project is fetched (only if we didn't have it)
	useEffect(() => {
		if (fetchedProject && shouldFetchProject) {
			updateCurrentProject(fetchedProject);
		}
	}, [fetchedProject, shouldFetchProject, updateCurrentProject]);

	if (shouldFetchOrg && isOrgLoading || shouldFetchProject && isProjectLoading) {
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
				<Typography variant="h4" component="h1" sx={{ mb: 2, fontWeight: 'bold' }}>
					{project ? project.name : `Project: ${projectSlug}`}
				</Typography>

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
							environment={project.environments[selectedTab]}
							flags={localFlags}
							isFlagsLoading={isFlagsLoading}
							isFlagsError={isFlagsError}
							flagsError={flagsError}
							processingFlags={processingFlags}
							onFlagToggle={handleFlagToggle}
							orgSlug={orgSlug}
							projectSlug={projectSlug}
						/>
					</Box>
				)}

				{!project && !shouldFetchProject && (
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
		</Container>
	);
}
export default Project;
