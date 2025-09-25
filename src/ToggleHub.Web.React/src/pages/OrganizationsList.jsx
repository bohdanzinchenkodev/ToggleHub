import React from "react";
import {
	Box,
	Typography,
	Grid,
	Container
} from "@mui/material";
import { useGetOrganizationsByCurrentUserQuery, useCreateOrganizationMutation } from "../redux/slices/apiSlice.js";
import { useNavigate } from "react-router";
import { useFormHandler } from "../hooks/useFormHandler.js";
import { useAppState } from "../hooks/useAppState.js";
import { validateForm } from "../utils/validation.js";
import InfiniteItemsList from "../components/shared/InfiniteItemsList.jsx";
import CreateForm from "../components/shared/CreateForm.jsx";
import useInfiniteScrollQuery from "../hooks/useInfiniteScrollQuery.js";
import { PAGINATION_CONFIG } from "../constants/organizationConstants.js";

const OrganizationsList = () => {
	const navigate = useNavigate();
	const { updateCurrentOrganization } = useAppState();
	
	// Use the reusable infinite scroll hook
	const {
		allItems: allOrganizations,
		totalCount: organizationsTotal,
		isLoading: isOrganizationsLoading,
		isError: isOrganizationsError,
		error: organizationsError,
		hasNextPage: hasMore,
		isFetchingNextPage,
		loadingRef,
		refetch: refetchOrganizations
	} = useInfiniteScrollQuery({
		useQuery: useGetOrganizationsByCurrentUserQuery,
		baseQueryParams: { pageSize: PAGINATION_CONFIG.DEFAULT_PAGE_SIZE }
	});

	const [createOrganization, { isLoading: isCreating, error: createError, isError: isCreateError }] = useCreateOrganizationMutation();

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

		resetForm();

		updateCurrentOrganization(response);

		// Reset pagination and refetch
		refetchOrganizations();

		if (response.slug) {
			navigate(`/organizations/${response.slug}`);
		}
		} catch (error) {
			console.error("Failed to create organization:", error);
			handleServerErrors(error);
		}
	};

	const handleOrganizationClick = (org) => {
		updateCurrentOrganization(org);
	};
	console.log(hasMore)
	return (
		<Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
			<Box>
				<Typography variant="h4" component="h1" sx={{ mb: 4, fontWeight: 'bold', textAlign: 'center' }}>
					Organizations
				</Typography>

				<Grid container spacing={4}>
					{/* Left Column - Available Organizations */}
					<Grid item size={{xs: 12, md: 6}}>
						<InfiniteItemsList
							title="Your Organizations"
							items={allOrganizations}
							totalCount={organizationsTotal}
							isLoading={isOrganizationsLoading}
							isError={isOrganizationsError}
							error={organizationsError}
							emptyMessage="No organizations found. Create your first organization to get started!"
							getItemLink={(org) => `/organizations/${org.slug}`}
							onItemClick={handleOrganizationClick}
							hasNextPage={hasMore}
							isFetchingNextPage={isFetchingNextPage}
							loadingRef={loadingRef}
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
