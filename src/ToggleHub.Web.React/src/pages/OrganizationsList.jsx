import React from "react";
import {
	Box,
	Typography,
	Grid,
	CircularProgress,
	Alert,
	Container
} from "@mui/material";
import { useGetOrganizationsByCurrentUserQuery, useCreateOrganizationMutation } from "../redux/slices/apiSlice.js";
import { useNavigate } from "react-router";
import { useFormHandler } from "../hooks/useFormHandler.js";
import { useAppState } from "../hooks/useAppState.js";
import { validateForm } from "../utils/validation.js";
import ItemsList from "../components/shared/ItemsList.jsx";
import CreateForm from "../components/shared/CreateForm.jsx";

const OrganizationsList = () => {
	const navigate = useNavigate();
	const { updateCurrentOrganization } = useAppState();
	const { data: organizations, isLoading, isError, error } = useGetOrganizationsByCurrentUserQuery();
	const [createOrganization, { isLoading: isCreating, error: createError, isError: isCreateError }] = useCreateOrganizationMutation();

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
			name: { required: true, label: "Organization name" }
		});

		if (Object.keys(validationErrors).length > 0) {
			setErrors(validationErrors);
			return;
		}

		try {
			const response = await createOrganization({
				name: formData.name.trim()
			}).unwrap();

			// Reset form
			resetForm();

			// Set the newly created organization in global state
			updateCurrentOrganization(response);

			// Redirect to the newly created organization
			if (response.slug) {
				navigate(`/organizations/${response.slug}`);
			}
		} catch (error) {
			console.error("Failed to create organization:", error);
			handleServerErrors(error);
		}
	};

	const handleOrganizationClick = (org) => {
		// Set organization in global state when user clicks on it
		updateCurrentOrganization(org);
	};

	if (isLoading) {
		return (
			<Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
				<CircularProgress />
			</Box>
		);
	}

	if (isError) {
		return (
			<Box sx={{ p: 3 }}>
				<Alert severity="error">
					Failed to load organizations: {error?.data?.detail || 'Unknown error'}
				</Alert>
			</Box>
		);
	}

	return (
		<Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
			<Box>
				<Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 'bold', textAlign: 'center' }}>
					Organizations
				</Typography>

				<Grid container spacing={4}>
					{/* Left Column - Available Organizations */}
					<Grid item size={{xs: 12, md: 6}}>
						<ItemsList
							title="Your Organizations"
							items={organizations}
							isLoading={isLoading}
							isError={isError}
							error={error}
							emptyMessage="No organizations found. Create your first organization to get started!"
							getItemLink={(org) => `/organizations/${org.slug}`}
							onItemClick={handleOrganizationClick}
						/>
					</Grid>

					{/* Right Column - Create New Organization */}
					<Grid item size={{xs: 12, md: 6}}>
						<CreateForm
							title="Create New Organization"
							subtitle="Start a new organization to collaborate with your team."
							formData={formData}
							formErrors={formErrors}
							isCreating={isCreating}
							isCreateError={isCreateError}
							createError={createError}
							onInputChange={handleInputChange}
							onSubmit={handleSubmit}
							inputLabel="Organization Name"
							inputName="name"
							buttonText="Create Organization"
						/>
					</Grid>
				</Grid>
			</Box>
		</Container>
	);
};

export default OrganizationsList;
