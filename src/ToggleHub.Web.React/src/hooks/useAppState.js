import { useParams } from 'react-router';
import { useDispatch, useSelector } from 'react-redux';
import {
	setCurrentOrganization,
	setCurrentProject,
	clearCurrentOrganization,
	clearCurrentProject,
	clearAppState,
	selectCurrentOrganization,
	selectCurrentProject,
} from '../redux/slices/appStateSlice';
import {
	useGetOrganizationBySlugQuery,
	useGetProjectBySlugQuery
} from '../redux/slices/apiSlice';
import { useEffect } from 'react';

export const useAppState = () => {
	const dispatch = useDispatch();
	const { orgSlug, projectSlug } = useParams();
	
	// Get cached values from Redux store
	const cachedOrganization = useSelector(selectCurrentOrganization);
	const cachedProject = useSelector(selectCurrentProject);

	// Fetch organization data based on URL parameter
	const {
		data: organizationData,
		isLoading: isOrgLoading,
		isError: isOrgError,
		error: orgError
	} = useGetOrganizationBySlugQuery(orgSlug, {
		skip: !orgSlug, // Skip if no orgSlug in URL
	});

	// Fetch project data based on URL parameter and organization
	const {
		data: projectData,
		isLoading: isProjectLoading,
		isError: isProjectError,
		error: projectError
	} = useGetProjectBySlugQuery(
		{
			projectSlug,
			organizationId: organizationData?.id || cachedOrganization?.id
		},
		{
			skip: !projectSlug || (!organizationData?.id && !cachedOrganization?.id), // Skip if no projectSlug or organization
		}
	);

	// Update Redux store when organization data is fetched
	useEffect(() => {
		if (organizationData && organizationData.id !== cachedOrganization?.id) {
			dispatch(setCurrentOrganization(organizationData));
		}
	}, [organizationData, cachedOrganization?.id, dispatch]);

	// Update Redux store when project data is fetched
	useEffect(() => {
		if (projectData && projectData.id !== cachedProject?.id) {
			dispatch(setCurrentProject(projectData));
		}
	}, [projectData, cachedProject?.id, dispatch]);

	// Clear organization from store when orgSlug changes or is removed
	useEffect(() => {
		if (!orgSlug && cachedOrganization) {
			dispatch(clearCurrentOrganization());
		}
	}, [orgSlug, cachedOrganization, dispatch]);

	// Clear project from store when projectSlug changes or is removed
	useEffect(() => {
		if (!projectSlug && cachedProject) {
			dispatch(clearCurrentProject());
		}
	}, [projectSlug, cachedProject, dispatch]);

	// Manual update functions (for backward compatibility or special cases)
	const updateCurrentOrganization = (organization) => {
		dispatch(setCurrentOrganization(organization));
	};

	const updateCurrentProject = (project) => {
		dispatch(setCurrentProject(project));
	};

	// Determine current values (prefer cached data for performance)
	const currentOrganization = cachedOrganization || organizationData;
	const currentProject = cachedProject || projectData;

	return {
		// Current data
		currentOrganization,
		currentProject,
		
		// URL parameters
		orgSlug,
		projectSlug,
		
		// Loading states
		isLoadingOrganization: isOrgLoading,
		isLoadingProject: isProjectLoading,
		isLoading: isOrgLoading || isProjectLoading,
		
		// Error states
		organizationError: orgError,
		projectError: projectError,
		isOrganizationError: isOrgError,
		isProjectError: isProjectError,
		hasError: isOrgError || isProjectError,
		
		// Manual update functions (for backward compatibility)
		updateCurrentOrganization,
		updateCurrentProject,
	};
};
