import React from 'react';
import {
	Card,
	CardContent,
	Typography,
	Alert,
	TextField,
	Button,
	CircularProgress,
	Box
} from "@mui/material";
import {
	Add as AddIcon
} from "@mui/icons-material";

const CreateForm = ({
	title,
	subtitle,
	formData,
	formErrors,
	isCreating,
	isCreateError,
	createError,
	onInputChange,
	onSubmit,
	inputLabel,
	inputName = "name",
	buttonText,
	placeholder
}) => {
	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 3, textAlign: 'center' }}>
					{title}
				</Typography>

				{subtitle && (
					<Typography variant="body2" color="text.secondary" sx={{ mb: 3, textAlign: 'center' }}>
						{subtitle}
					</Typography>
				)}

				{isCreateError && (
					<Alert severity="error" sx={{ mb: 2 }}>
						{(createError?.status !== 500 && createError?.data?.detail) || "Something went wrong. Please try again."}
					</Alert>
				)}

				<Box component="form" onSubmit={onSubmit}>
					<TextField
						fullWidth
						label={inputLabel}
						name={inputName}
						value={formData[inputName] || ""}
						onChange={onInputChange}
						error={!!formErrors[inputName]}
						helperText={formErrors[inputName]}
						disabled={isCreating}
						sx={{ mb: 3 }}
						placeholder={placeholder}
					/>

					<Button
						type="submit"
						fullWidth
						variant="contained"
						disabled={isCreating}
						startIcon={isCreating ? <CircularProgress size={20} /> : <AddIcon />}
						sx={{ mt: 2 }}
					>
						{isCreating ? 'Creating...' : buttonText}
					</Button>
				</Box>
			</CardContent>
		</Card>
	);
};

export default CreateForm;
