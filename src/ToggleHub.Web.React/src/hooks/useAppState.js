import { useParams } from 'react-router';
import { useDispatch, useSelector } from 'react-redux';
import {
	setCurrentOrganization,
	setCurrentProject,
	setCurrentUserPermissions,
	clearCurrentOrganization,
	clearCurrentProject,
	clearCurrentUserPermissions,
	clearAppState,
	selectCurrentOrganization,
	selectCurrentProject,
	selectCurrentUserPermissions,
} from '../redux/slices/appStateSlice';
import {
	useGetOrganizationBySlugQuery,
	useGetProjectBySlugQuery,
	useGetUserPermissionsQuery
} from '../redux/slices/apiSlice';
import { useEffect } from 'react';

export const useAppState = () => {
	const dispatch = useDispatch();
	const { orgSlug, projectSlug } = useParams();
	
	// Get cached values from Redux store
	const cachedOrganization = useSelector(selectCurrentOrganization);
	const cachedProject = useSelector(selectCurrentProject);
	const cachedUserPermissions = useSelector(selectCurrentUserPermissions);

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

	// Fetch user permissions for current organization
	const {
		data: userPermissionsData,
		isLoading: isPermissionsLoading,
		isError: isPermissionsError,
		error: permissionsError
	} = useGetUserPermissionsQuery(
		organizationData?.id || cachedOrganization?.id,
		{
			skip: !organizationData?.id && !cachedOrganization?.id, // Skip if no organization
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

	// Update Redux store when permissions data is fetched
	useEffect(() => {
		if (userPermissionsData) {
			dispatch(setCurrentUserPermissions(userPermissionsData));
		}
	}, [userPermissionsData, dispatch]);

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

	const updateCurrentUserPermissions = (permissions) => {
		dispatch(setCurrentUserPermissions(permissions));
	};

	// Determine current values (prefer cached data for performance)
	const currentOrganization = cachedOrganization || organizationData;
	const currentProject = cachedProject || projectData;
	const currentUserPermissions = cachedUserPermissions || userPermissionsData;

	return {
		// Current data
		currentOrganization,
		currentProject,
		currentUserPermissions,
		
		// URL parameters
		orgSlug,
		projectSlug,
		
		// Loading states
		isLoadingOrganization: isOrgLoading,
		isLoadingProject: isProjectLoading,
		isLoadingPermissions: isPermissionsLoading,
		isLoading: isOrgLoading || isProjectLoading || isPermissionsLoading,
		
		// Error states
		organizationError: orgError,
		projectError: projectError,
		permissionsError: permissionsError,
		isOrganizationError: isOrgError,
		isProjectError: isProjectError,
		isPermissionsError: isPermissionsError,
		hasError: isOrgError || isProjectError || isPermissionsError,
		
		// Manual update functions (for backward compatibility)
		updateCurrentOrganization,
		updateCurrentProject,
		updateCurrentUserPermissions,
	};
};
