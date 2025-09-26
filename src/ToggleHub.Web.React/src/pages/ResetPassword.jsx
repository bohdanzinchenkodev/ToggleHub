import React, { useState, useEffect } from "react";
import {
	Box,
	Card,
	CardContent,
	TextField,
	Button,
	Typography,
	Container,
	InputAdornment,
	IconButton,
	Alert,
	Link,
	CircularProgress
} from "@mui/material";
import {
	Visibility,
	VisibilityOff,
	Email,
	Lock,
	ArrowBack
} from "@mui/icons-material";
import { useResetPasswordMutation } from "../redux/slices/apiSlice.js";
import { Link as RouterLink, useSearchParams, useNavigate } from "react-router";
import { useAuthForm } from "../hooks/useAuthForm.js";

const ResetPassword = () => {
	const [searchParams] = useSearchParams();
	const navigate = useNavigate();
	const token = searchParams.get('token');
	const emailParam = searchParams.get('email');
	
	const { formData, errors, handleChange, validateResetPasswordForm, setFormData } = useAuthForm({
		email: emailParam || "",
		token: token || "",
		newPassword: "",
		confirmPassword: ""
	});
	
	const [showPassword, setShowPassword] = useState(false);
	const [showConfirmPassword, setShowConfirmPassword] = useState(false);
	const [isSubmitted, setIsSubmitted] = useState(false);
	const [resetPassword, { error, isError, isLoading }] = useResetPasswordMutation();

	// Set email and token from URL params when component mounts
	useEffect(() => {
		if (emailParam || token) {
			setFormData(prev => ({
				...prev,
				email: emailParam || prev.email,
				token: token || prev.token
			}));
		}
	}, [emailParam, token, setFormData]);

	// Redirect if no token is provided
	useEffect(() => {
		if (!token) {
			navigate('/forgot-password');
		}
	}, [token, navigate]);

	const handleSubmit = async (e) => {
		e.preventDefault();
		
		if (!validateResetPasswordForm()) {
			return;
		}

		try {
			await resetPassword({
				email: formData.email,
				token: formData.token,
				newPassword: formData.newPassword,
				confirmPassword: formData.confirmPassword
			}).unwrap();
			setIsSubmitted(true);
		} catch (err) {
			console.error("Reset password failed:", err);
		}
	};

	const togglePasswordVisibility = () => {
		setShowPassword(!showPassword);
	};

	const toggleConfirmPasswordVisibility = () => {
		setShowConfirmPassword(!showConfirmPassword);
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
								Password Reset Successful
							</Typography>

							<Alert severity="success" sx={{ mb: 3 }}>
								Your password has been reset successfully!
							</Alert>

							<Typography variant="body1" align="center" sx={{ mb: 3 }}>
								You can now sign in with your new password.
							</Typography>

							<Button
								component={RouterLink}
								to="/login"
								fullWidth
								variant="contained"
								sx={{ mt: 2, height: 48 }}
							>
								Go to Sign In
							</Button>
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
								Reset Password
							</Typography>

							<Typography
								variant="body1"
								align="center"
								color="text.secondary"
								sx={{ mb: 3 }}
							>
								Enter your new password below.
							</Typography>

							{isError && (
								<Alert severity="error" sx={{ mb: 2 }}>
									{error?.data?.detail || "Invalid or expired reset token. Please request a new password reset."}
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
								value={formData.email}
								onChange={handleChange}
								error={!!errors.email}
								helperText={errors.email}
								disabled={!!emailParam} // Disable if email comes from URL
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

							<TextField
								margin="normal"
								required
								fullWidth
								name="newPassword"
								label="New Password"
								type={showPassword ? "text" : "password"}
								id="newPassword"
								autoComplete="new-password"
								autoFocus={!!emailParam} // Auto focus if email is pre-filled
								value={formData.newPassword}
								onChange={handleChange}
								error={!!errors.newPassword}
								helperText={errors.newPassword}
								slotProps={{
									input: {
										startAdornment: (
											<InputAdornment position="start">
												<Lock />
											</InputAdornment>
										),
										endAdornment: (
											<InputAdornment position="end">
												<IconButton
													aria-label="toggle password visibility"
													onClick={togglePasswordVisibility}
													edge="end"
												>
													{showPassword ? <VisibilityOff /> : <Visibility />}
												</IconButton>
											</InputAdornment>
										),
									},
								}}
							/>

							<TextField
								margin="normal"
								required
								fullWidth
								name="confirmPassword"
								label="Confirm New Password"
								type={showConfirmPassword ? "text" : "password"}
								id="confirmPassword"
								autoComplete="new-password"
								value={formData.confirmPassword}
								onChange={handleChange}
								error={!!errors.confirmPassword}
								helperText={errors.confirmPassword}
								slotProps={{
									input: {
										startAdornment: (
											<InputAdornment position="start">
												<Lock />
											</InputAdornment>
										),
										endAdornment: (
											<InputAdornment position="end">
												<IconButton
													aria-label="toggle confirm password visibility"
													onClick={toggleConfirmPasswordVisibility}
													edge="end"
												>
													{showConfirmPassword ? <VisibilityOff /> : <Visibility />}
												</IconButton>
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
								{isLoading ? "Resetting..." : "Reset Password"}
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

export default ResetPassword;
