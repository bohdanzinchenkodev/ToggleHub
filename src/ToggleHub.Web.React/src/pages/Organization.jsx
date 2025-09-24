import React from 'react';
import {
	Box,
	Typography,
	Grid,
	CircularProgress,
	Alert,
	Container,
	Button
} from "@mui/material";
import { ArrowBack as ArrowBackIcon, People as PeopleIcon } from '@mui/icons-material';
import { Link } from 'react-router';
import {
	useGetProjectsByOrganizationQuery,
	useCreateProjectMutation
} from "../redux/slices/apiSlice.js";
import { useFormHandler } from "../hooks/useFormHandler.js";
import { useAppState } from "../hooks/useAppState.js";
import { validateForm } from "../utils/validation.js";
import InfiniteItemsList from "../components/shared/InfiniteItemsList.jsx";
import CreateForm from "../components/shared/CreateForm.jsx";
import AppStateDisplay from "../components/shared/AppStateDisplay.jsx";
import useInfiniteScrollQuery from "../hooks/useInfiniteScrollQuery.js";
import { PAGINATION_CONFIG } from "../constants/organizationConstants.js";

const Organization = () => {
	const {
		currentOrganization: organization,
		orgSlug,
		isLoadingOrganization: isOrgLoading,
		isOrganizationError: isOrgError,
		organizationError: orgError,
		updateCurrentProject
	} = useAppState();

	// Use the reusable infinite scroll hook
	const {
		allItems: allProjects,
		isLoading: isProjectsLoading,
		isError: isProjectsError,
		error: projectsError,
		hasNextPage: hasMore,
		isFetchingNextPage,
		loadingRef,
		refetch: refetchProjects
	} = useInfiniteScrollQuery({
		useQuery: useGetProjectsByOrganizationQuery,
		baseQueryParams: {
			organizationId: organization?.id,
			pageSize: PAGINATION_CONFIG.DEFAULT_PAGE_SIZE
		},
		skipCondition: !organization?.id
	});

	const [createProject, {
		isLoading: isCreating,
		error: createError,
		isError: isCreateError
	}] = useCreateProjectMutation();

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

			resetForm();
			// Reset pagination and refetch
			refetchProjects();
		} catch (error) {
			console.error('Failed to create project:', error);
			handleServerErrors(error);
		}
	};

	const handleProjectClick = (project) => {
		updateCurrentProject(project);
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
			<Box sx={{ mb: 3, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
				<Box sx={{ display: 'flex', alignItems: 'center', gap: 2, minHeight: '40px' }}>
					<Box sx={{
						'& > *': { mb: 0 },
						display: 'flex',
						alignItems: 'center'
					}}>
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
				<Button
					startIcon={<PeopleIcon />}
					component={Link}
					to={`/organizations/${orgSlug}/members`}
					variant="contained"
				>
					Manage Members
				</Button>
			</Box>

			<Box>
				<Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 'bold', textAlign: 'center' }}>
					{organization?.name}
				</Typography>

				<Grid container spacing={4}>
					{/* Left Column - Available Projects */}
					<Grid item size={{xs: 12, md: 6}}>
						<InfiniteItemsList
							title="Projects"
							items={allProjects}
							isLoading={isProjectsLoading}
							isError={isProjectsError}
							error={projectsError}
							emptyMessage="No projects found. Create your first project to get started!"
							getItemLink={(project) => `/organizations/${orgSlug}/projects/${project.slug}`}
							onItemClick={handleProjectClick}
							hasNextPage={hasMore}
							isFetchingNextPage={isFetchingNextPage}
							loadingRef={loadingRef}
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
