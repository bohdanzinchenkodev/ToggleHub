import React from 'react';
import {
	Box,
	Typography,
	Grid,
	CircularProgress,
	Alert,
	Container
} from "@mui/material";
import { useParams } from "react-router";
import {
	useGetOrganizationBySlugQuery,
	useGetProjectsByOrganizationQuery,
	useCreateProjectMutation
} from "../redux/slices/apiSlice.js";
import { useFormHandler } from "../hooks/useFormHandler.js";
import { validateForm } from "../utils/validation.js";
import ItemsList from "../components/shared/ItemsList.jsx";
import CreateForm from "../components/shared/CreateForm.jsx";

const Organization = () => {
	const { slug } = useParams();

	// Get organization details by slug
	const {
		data: organization,
		isLoading: isOrgLoading,
		isError: isOrgError,
		error: orgError
	} = useGetOrganizationBySlugQuery(slug);

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

	if (isOrgLoading) {
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
