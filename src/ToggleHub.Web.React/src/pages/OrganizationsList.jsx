import React, { useState } from "react";
import {
	Box,
	Typography,
	Card,
	CardContent,
	CardActions,
	Button,
	TextField,
	Grid,
	CircularProgress,
	Alert,
	Chip,
	IconButton, Container
} from "@mui/material";
import {
	Add as AddIcon,
	Edit as EditIcon,
	Delete as DeleteIcon,
	Visibility as ViewIcon
} from "@mui/icons-material";
import { useGetOrganizationsByCurrentUserQuery, useCreateOrganizationMutation } from "../redux/slices/apiSlice.js";
import { useNavigate, Link } from "react-router";

const OrganizationsList = () => {
	const navigate = useNavigate();
	const { data: organizations, isLoading, isError, error } = useGetOrganizationsByCurrentUserQuery();
	const [createOrganization, { isLoading: isCreating, error: createError, isError: isCreateError }] = useCreateOrganizationMutation();

	const [formData, setFormData] = useState({
		name: ""
	});
	const [formErrors, setFormErrors] = useState({});

	const handleInputChange = (e) => {
		const { name, value } = e.target;
		setFormData(prev => ({
			...prev,
			[name]: value
		}));
		// Clear error when user starts typing
		if (formErrors[name]) {
			setFormErrors(prev => ({
				...prev,
				[name]: ""
			}));
		}
	};

	const handleSubmit = async (e) => {
		e.preventDefault();
		const newErrors = {};

		// Validation
		if (!formData.name.trim()) {
			newErrors.name = "Organization name is required";
		}

		setFormErrors(newErrors);

		if (Object.keys(newErrors).length === 0) {
			try {
				const response = await createOrganization({
					name: formData.name.trim()
				}).unwrap();

				// Reset form
				setFormData({ name: "" });
				
				// Redirect to the newly created organization
				if (response.slug) {
					navigate(`/organizations/${response.slug}`);
				}
			} catch (error) {
				console.error("Failed to create organization:", error);

				// Handle server validation errors
				if (error?.status === 400 && error?.data?.errors) {
					const serverErrors = {};
					Object.keys(error.data.errors).forEach(field => {
						const errorMessages = error.data.errors[field];
						if (Array.isArray(errorMessages) && errorMessages.length > 0) {
							serverErrors[field] = errorMessages[0]; // Take the first error message
						}
					});
					setFormErrors(serverErrors);
				}
			}
		}
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
					<Grid item size={{xs: 12, md: 6}} >
						<Card>
							<CardContent>
								<Typography variant="h6" sx={{ mb: 2, textAlign: 'center' }}>
									Your Organizations ({organizations?.length || 0})
								</Typography>

								{organizations?.length === 0 ? (
									<Card>
										<CardContent>
											<Typography color="text.secondary" textAlign="center"  sx={{ whiteSpace: "normal", wordBreak: "break-word" }}>
												No organizations found. Create your first organization to get started!
											</Typography>
										</CardContent>
									</Card>
								) : (
									<Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, p: 2 }}>
										{organizations?.map((org) => (
											<Link
												key={org.id}
												to={`/organizations/${org.slug}`}
												style={{ textDecoration: 'none' }}
											>
												<Typography
													variant="body1"
													sx={{
														color: 'primary.main',
														cursor: 'pointer',
														'&:hover': {
															textDecoration: 'underline'
														}
													}}
												>
													{org.name}
												</Typography>
											</Link>
										))}
									</Box>
								)}
							</CardContent>
						</Card>
					</Grid>

					{/* Right Column - Create New Organization */}
					<Grid item size={{xs: 12, md: 6}}  >
						<Card >
							<CardContent>
								<Typography variant="h6" sx={{ mb: 2, textAlign: 'center' }}>
									Create New Organization
								</Typography>
								<Typography variant="body2" color="text.secondary" sx={{ mb: 3, textAlign: 'center' }}>
									Start a new organization to collaborate with your team.
								</Typography>

								{isCreateError && (
									<Alert severity="error" sx={{ mb: 2 }}>
										{(createError.status !== 500 && createError?.data?.detail) || "Something went wrong. Please try again."}
									</Alert>
								)}

								{Object.keys(formErrors).length > 0 && (
									<Alert severity="error" sx={{ mb: 2 }}>
										Please fix the errors below and try again.
									</Alert>
								)}

								<form onSubmit={handleSubmit}>
									<TextField
										name="name"
										label="Organization Name"
										fullWidth
										variant="outlined"
										value={formData.name}
										onChange={handleInputChange}
										error={!!formErrors.name}
										helperText={formErrors.name}
										sx={{ mb: 2 }}
									/>
									<Button
										type="submit"
										variant="contained"
										startIcon={isCreating ? <CircularProgress size={20} /> : <AddIcon />}
										fullWidth
										disabled={isCreating}
										sx={{ mb: 2 }}
									>
										{isCreating ? 'Creating...' : 'Create Organization'}
									</Button>
								</form>
							</CardContent>
						</Card>
					</Grid>
				</Grid>
			</Box>
		</Container>
	);
};

export default OrganizationsList;
