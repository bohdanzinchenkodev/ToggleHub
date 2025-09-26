import React, { useState } from "react";
import {
	Box,
	Card,
	CardContent,
	TextField,
	Button,
	Typography,
	Container,
	InputAdornment,
	Alert,
	Link,
	CircularProgress
} from "@mui/material";
import {
	Email,
	ArrowBack
} from "@mui/icons-material";
import { useForgotPasswordMutation } from "../redux/slices/apiSlice.js";
import { Link as RouterLink } from "react-router";
import { useAuthForm } from "../hooks/useAuthForm.js";

const ForgotPassword = () => {
	const { formData, errors, handleChange, validateForgotPasswordForm } = useAuthForm({
		email: ""
	});
	const [isSubmitted, setIsSubmitted] = useState(false);
	const [forgotPassword, { error, isError, isLoading }] = useForgotPasswordMutation();

	const handleSubmit = async (e) => {
		e.preventDefault();
		
		if (!validateForgotPasswordForm()) {
			return;
		}

		try {
			await forgotPassword({ email: formData.email }).unwrap();
			setIsSubmitted(true);
		} catch (err) {
			console.error("Forgot password failed:", err);
		}
	};

	if (isSubmitted) {
		return (
			<Container component="main" maxWidth="sm">
				<Box
					sx={{
						marginTop: 8,
						display: "flex",
						flexDirection: "column",
						alignItems: "center",
					}}
				>
					<Card sx={{ width: "100%", maxWidth: 400 }}>
						<CardContent sx={{ p: 4 }}>
							<Typography
								component="h1"
								variant="h4"
								align="center"
								sx={{ mb: 3, fontWeight: "bold" }}
							>
								Check Your Email
							</Typography>

							<Alert severity="success" sx={{ mb: 3 }}>
								If an account with that email exists, a password reset link has been sent.
							</Alert>

							<Typography variant="body1" align="center" sx={{ mb: 3 }}>
								Please check your email and click the reset link to continue. The link will expire in 1 hour.
							</Typography>

							<Box sx={{ textAlign: "center", mt: 3 }}>
								<Typography variant="body2">
									Remember your password?{" "}
									<Link component={RouterLink} to="/login" variant="body2">
										Back to Sign In
									</Link>
								</Typography>
							</Box>
						</CardContent>
					</Card>
				</Box>
			</Container>
		);
	}

	return (
		<Container component="main" maxWidth="sm">
			<Box
				sx={{
					marginTop: 8,
					display: "flex",
					flexDirection: "column",
					alignItems: "center",
				}}
			>
				<Card sx={{ width: "100%", maxWidth: 400 }}>
					<CardContent sx={{ p: 4 }}>
						<Box
							component="form"
							onSubmit={handleSubmit}
							sx={{ width: "100%" }}
						>
							<Typography
								component="h1"
								variant="h4"
								align="center"
								sx={{ mb: 3, fontWeight: "bold" }}
							>
								Forgot Password
							</Typography>

							<Typography
								variant="body1"
								align="center"
								color="text.secondary"
								sx={{ mb: 3 }}
							>
								Enter your email address and we'll send you a link to reset your password.
							</Typography>

							{isError && (
								<Alert severity="error" sx={{ mb: 2 }}>
									{error?.data?.detail || "Something went wrong. Please try again."}
								</Alert>
							)}

							<TextField
								margin="normal"
								required
								fullWidth
								id="email"
								label="Email Address"
								name="email"
								autoComplete="email"
								autoFocus
								value={formData.email}
								onChange={handleChange}
								error={!!errors.email}
								helperText={errors.email}
								slotProps={{
									input: {
										startAdornment: (
											<InputAdornment position="start">
												<Email />
											</InputAdornment>
										),
									},
								}}
							/>

							<Button
								type="submit"
								fullWidth
								variant="contained"
								disabled={isLoading}
								sx={{ mt: 3, mb: 2, height: 48 }}
								startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : null}
							>
								{isLoading ? "Sending..." : "Send Reset Link"}
							</Button>

							<Box sx={{ textAlign: "center", mt: 2 }}>
								<Link 
									component={RouterLink} 
									to="/login"
									variant="body2"
									sx={{ 
										display: "inline-flex", 
										alignItems: "center", 
										gap: 0.5 
									}}
								>
									<ArrowBack fontSize="small" />
									Back to Sign In
								</Link>
							</Box>
						</Box>
					</CardContent>
				</Card>
			</Box>
		</Container>
	);
};

export default ForgotPassword;
