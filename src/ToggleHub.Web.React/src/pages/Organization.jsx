import React, { useEffect } from 'react';
import {
	Box,
	Typography,
	Grid,
	CircularProgress,
	Alert,
	Container,
	Button
} from "@mui/material";
import { useParams } from "react-router";
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import { Link } from 'react-router';
import {
	useGetOrganizationBySlugQuery,
	useGetProjectsByOrganizationQuery,
	useCreateProjectMutation
} from "../redux/slices/apiSlice.js";
import { useFormHandler } from "../hooks/useFormHandler.js";
import { useAppState } from "../hooks/useAppState.js";
import { validateForm } from "../utils/validation.js";
import ItemsList from "../components/shared/ItemsList.jsx";
import CreateForm from "../components/shared/CreateForm.jsx";
import AppStateDisplay from "../components/shared/AppStateDisplay.jsx";

const Organization = () => {
	const { slug } = useParams();
	const { currentOrganization, updateCurrentOrganization, updateCurrentProject, clearProject } = useAppState();

	// Only fetch organization if we don't have it in state or if the slug doesn't match
	const shouldFetchOrg = !currentOrganization || currentOrganization.slug !== slug;

	// Get organization details by slug (only if needed)
	const {
		data: fetchedOrganization,
		isLoading: isOrgLoading,
		isError: isOrgError,
		error: orgError
	} = useGetOrganizationBySlugQuery(slug, {
		skip: !shouldFetchOrg
	});

	// Use organization from state if available, otherwise use fetched data
	const organization = currentOrganization?.slug === slug ? currentOrganization : fetchedOrganization;

	// Update global state when organization is fetched (only if we didn't have it)
	useEffect(() => {
		if (fetchedOrganization && shouldFetchOrg) {
			updateCurrentOrganization(fetchedOrganization);
		}
	}, [fetchedOrganization, shouldFetchOrg, updateCurrentOrganization]);

	// Clear current project when component unmounts or organization changes
	useEffect(() => {
		return () => {
			clearProject();
		};
	}, [clearProject, slug]);

	// Get projects for this organization
	const {
		data: projectsResponse,
		isLoading: isProjectsLoading,
		isError: isProjectsError,
		error: projectsError,
		refetch: refetchProjects
	} = useGetProjectsByOrganizationQuery(organization?.id, {
		skip: !organization?.id
	});

	// Extract projects array from the paginated response
	const projects = projectsResponse?.data || [];

	// Create project mutation
	const [createProject, {
		isLoading: isCreating,
		error: createError,
		isError: isCreateError
	}] = useCreateProjectMutation();

	// Form handling
	const {
		formData,
		formErrors,
		handleInputChange,
		handleServerErrors,
		resetForm,
		setErrors
	} = useFormHandler({ name: "" });

	const handleSubmit = async (e) => {
		e.preventDefault();

		// Validation
		const validationErrors = validateForm(formData, {
			name: { required: true, label: "Project name" }
		});

		if (Object.keys(validationErrors).length > 0) {
			setErrors(validationErrors);
			return;
		}

		try {
			await createProject({
				organizationId: organization.id,
				body: {
					name: formData.name.trim()
				}
			}).unwrap();

			// Reset form and refetch projects
			resetForm();
			refetchProjects();
		} catch (error) {
			console.error('Failed to create project:', error);
			handleServerErrors(error);
		}
	};

	const handleProjectClick = (project) => {
		// Set project in global state when user clicks on it
		updateCurrentProject(project);
	};

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
			<Box sx={{ mb: 3, display: 'flex', alignItems: 'center', justifyContent: 'flex-start', gap: 2 }}>
				<Box sx={{ '& > *': { mb: 0 } }}>
					<AppStateDisplay />
				</Box>
				<Button
					startIcon={<ArrowBackIcon />}
					component={Link}
					to="/"
					variant="outlined"
				>
					Back to Organizations
				</Button>
			</Box>

			<Box>
				<Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 'bold', textAlign: 'center' }}>
					{organization?.name}
				</Typography>

				<Grid container spacing={4}>
					{/* Left Column - Available Projects */}
					<Grid item size={{xs: 12, md: 6}}>
						<ItemsList
							title="Projects"
							items={projects}
							isLoading={isProjectsLoading}
							isError={isProjectsError}
							error={projectsError}
							emptyMessage="No projects found. Create your first project to get started!"
							getItemLink={(project) => `/organizations/${slug}/projects/${project.slug}`}
							onItemClick={handleProjectClick}
						/>
					</Grid>

					{/* Right Column - Create New Project Form */}
					<Grid item size={{xs: 12, md: 6}}>
						<CreateForm
							title="Create New Project"
							formData={formData}
							formErrors={formErrors}
							isCreating={isCreating}
							isCreateError={isCreateError}
							createError={createError}
							onInputChange={handleInputChange}
							onSubmit={handleSubmit}
							inputLabel="Project Name"
							inputName="name"
							buttonText="Create Project"
							placeholder="Enter project name"
						/>
					</Grid>
				</Grid>
			</Box>
		</Container>
	);
};

export default Organization;
