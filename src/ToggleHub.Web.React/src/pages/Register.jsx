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
	Person
} from "@mui/icons-material";
import { useRegisterMutation } from "../redux/slices/apiSlice.js";
import { useNavigate } from "react-router";

const Register = () => {
	const [formData, setFormData] = useState({
		firstName: "",
		lastName: "",
		email: "",
		password: "",
		confirmPassword: ""
	});
	const [showPassword, setShowPassword] = useState(false);
	const [showConfirmPassword, setShowConfirmPassword] = useState(false);
	const [errors, setErrors] = useState({});
	const [register, { error, isError, isLoading }] = useRegisterMutation();
	const navigate = useNavigate();

	const handleChange = (e) => {
		const { name, value } = e.target;
		setFormData(prev => ({
			...prev,
			[name]: value
		}));
		// Clear error when user starts typing
		if (errors[name]) {
			setErrors(prev => ({
				...prev,
				[name]: ""
			}));
		}
	};

	const handleSubmit = async (e) => {
		e.preventDefault();
		const newErrors = {};

		// Basic validation
		if (!formData.firstName) {
			newErrors.firstName = "First name is required";
		}

		if (!formData.lastName) {
			newErrors.lastName = "Last name is required";
		}

		if (!formData.email) {
			newErrors.email = "Email is required";
		} else if (!/\S+@\S+\.\S+/.test(formData.email)) {
			newErrors.email = "Email is invalid";
		}

		if (!formData.password) {
			newErrors.password = "Password is required";
		} else if (formData.password.length < 6) {
			newErrors.password = "Password must be at least 6 characters";
		}

		if (!formData.confirmPassword) {
			newErrors.confirmPassword = "Please confirm your password";
		} else if (formData.password !== formData.confirmPassword) {
			newErrors.confirmPassword = "Passwords do not match";
		}

		setErrors(newErrors);

		if (Object.keys(newErrors).length === 0) {
			try {
				await register({
					firstName: formData.firstName,
					lastName: formData.lastName,
					email: formData.email,
					password: formData.password
				}).unwrap();
				navigate("/");
			} catch (err) {
				console.error("Registration failed:", err);
			}
		}
	};

	const togglePasswordVisibility = () => {
		setShowPassword(!showPassword);
	};

	const toggleConfirmPasswordVisibility = () => {
		setShowConfirmPassword(!showConfirmPassword);
	};

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
								Sign Up
							</Typography>

							{isError && (
								<Alert severity="error" sx={{ mb: 2 }}>
									{error?.data?.detail || "Something went wrong. Please try again."}
								</Alert>
							)}

							<Box sx={{ display: "flex", gap: 1 }}>
								<TextField
									margin="normal"
									required
									fullWidth
									id="firstName"
									label="First Name"
									name="firstName"
									autoComplete="given-name"
									autoFocus
									value={formData.firstName}
									onChange={handleChange}
									error={!!errors.firstName}
									helperText={errors.firstName}
									slotProps={{
										input: {
											startAdornment: (
												<InputAdornment position="start">
													<Person />
												</InputAdornment>
											),
										},
									}}
								/>

								<TextField
									margin="normal"
									required
									fullWidth
									id="lastName"
									label="Last Name"
									name="lastName"
									autoComplete="family-name"
									value={formData.lastName}
									onChange={handleChange}
									error={!!errors.lastName}
									helperText={errors.lastName}
									slotProps={{
										input: {
											startAdornment: (
												<InputAdornment position="start">
													<Person />
												</InputAdornment>
											),
										},
									}}
								/>
							</Box>

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
								name="password"
								label="Password"
								type={showPassword ? "text" : "password"}
								id="password"
								autoComplete="new-password"
								value={formData.password}
								onChange={handleChange}
								error={!!errors.password}
								helperText={errors.password}
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
								label="Confirm Password"
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
								type={"submit"}
								fullWidth
								variant="contained"
								disabled={isLoading}
								sx={{ mt: 3, mb: 2, height: 48 }}
								startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : null}
							>
								{isLoading ? "Creating Account..." : "Sign Up"}
							</Button>

							<Box sx={{ textAlign: "center", mt: 2 }}>
								<Typography variant="body2">
									Already have an account?{" "}
									<Link href="/login" variant="body2">
										Sign in here
									</Link>
								</Typography>
							</Box>
						</Box>
					</CardContent>
				</Card>
			</Box>
		</Container>
	);
};

export default Register;
