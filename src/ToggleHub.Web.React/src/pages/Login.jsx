import React, { useState} from "react";
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
	Lock
} from "@mui/icons-material";
import {useLoginMutation} from "../redux/slices/apiSlice.js";
import {Link as RouterLink} from "react-router";
import { useAuthRedirect } from "../hooks/useAuthRedirect.js";
import { useAuthForm } from "../hooks/useAuthForm.js";

const Login = () => {
	const { formData, errors, handleChange, validateLoginForm } = useAuthForm({
		email: "",
		password: ""
	});
	const [showPassword, setShowPassword] = useState(false);
	const [login, { error, isError, isLoading }] = useLoginMutation();
	const { redirectAfterAuth, getAuthLinkUrl } = useAuthRedirect();


	const handleSubmit = async (e) => {
		e.preventDefault();
		
		if (!validateLoginForm()) {
			return;
		}

		try {
			await login({...formData}).unwrap();
			redirectAfterAuth();
		}
		catch (err) {
			console.log(err)
		}
	};

	const togglePasswordVisibility = () => {
		setShowPassword(!showPassword);
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
									Sign In
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

								<TextField
									margin="normal"
									required
									fullWidth
									name="password"
									label="Password"
									type={showPassword ? "text" : "password"}
									id="password"
									autoComplete="current-password"
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

								<Button
									type={"submit"}
									fullWidth
									variant="contained"
									disabled={isLoading}
									sx={{ mt: 3, mb: 2, height: 48 }}
									startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : null}
								>
									{isLoading ? "Signing In..." : "Sign In"}
								</Button>

								<Box sx={{ textAlign: "center", mt: 2 }}>
									<Typography variant="body2">
										Don't have an account?{" "}
										<Link 
											component={RouterLink} 
											to={getAuthLinkUrl('login')}
											variant="body2"
										>
											Sign up here
										</Link>
									</Typography>
								</Box>

								<Box sx={{ textAlign: "center", mt: 1 }}>
									<Link component={RouterLink} to="/forgot-password" variant="body2">
										Forgot your password?
									</Link>
								</Box>
							</Box>
						</CardContent>
					</Card>
				</Box>
			</Container>
	);
};

export default Login;
