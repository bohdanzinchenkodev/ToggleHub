import React, { useEffect } from 'react';
import {
	Box,
	Typography,
	CircularProgress,
	Alert,
	Container
} from "@mui/material";
import { useParams } from "react-router";
import {
	useGetOrganizationBySlugQuery
	// useGetProjectBySlugQuery // You'll need to add this to apiSlice
} from "../redux/slices/apiSlice.js";
import { useAppState } from "../hooks/useAppState.js";
import AppStateDisplay from "../components/shared/AppStateDisplay.jsx";

const Project = () => {
	const { orgSlug, projectSlug } = useParams();
	const { currentOrganization, currentProject, updateCurrentOrganization, updateCurrentProject } = useAppState();

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

	// TODO: Add project fetching logic when you implement getProjectBySlug API
	// Only fetch project if we don't have it in state or if the slug doesn't match
	// const shouldFetchProject = !currentProject || currentProject.slug !== projectSlug;
	
	// const {
	// 	data: fetchedProject,
	// 	isLoading: isProjectLoading,
	// 	isError: isProjectError,
	// 	error: projectError
	// } = useGetProjectBySlugQuery({ orgSlug, projectSlug }, {
	// 	skip: !shouldFetchProject || !organization?.id
	// });

	// const project = currentProject?.slug === projectSlug ? currentProject : fetchedProject;

	// Update global state when organization is fetched (only if we didn't have it)
	useEffect(() => {
		if (fetchedOrganization && shouldFetchOrg) {
			updateCurrentOrganization(fetchedOrganization);
		}
	}, [fetchedOrganization, shouldFetchOrg, updateCurrentOrganization]);

	// Update global state when project is fetched (only if we didn't have it)
	// useEffect(() => {
	// 	if (fetchedProject && shouldFetchProject) {
	// 		updateCurrentProject(fetchedProject);
	// 	}
	// }, [fetchedProject, shouldFetchProject, updateCurrentProject]);

	if (shouldFetchOrg && isOrgLoading) {
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

	return (
		<Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
			<AppStateDisplay />
			<Box>
				<Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 'bold', textAlign: 'center' }}>
					Project: {projectSlug}
				</Typography>
				
				{/* Add your project content here */}
				<Typography variant="body1" sx={{ textAlign: 'center' }}>
					Project content goes here...
				</Typography>
			</Box>
		</Container>
	);
}
export default Project;
