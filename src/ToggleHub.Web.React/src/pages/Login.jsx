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
import {useNavigate} from "react-router";

const Login = () => {
	const [formData, setFormData] = useState({
		email: "",
		password: ""
	});
	const [showPassword, setShowPassword] = useState(false);
	const [errors, setErrors] = useState({});
	const [login, { error, isError, isLoading }] = useLoginMutation();
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

		setErrors(newErrors);

		if (Object.keys(newErrors).length > 0)
			return;

		try {
			await login({...formData}).unwrap();
			navigate("/");
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
										<Link href="#" variant="body2">
											Sign up here
										</Link>
									</Typography>
								</Box>

								<Box sx={{ textAlign: "center", mt: 1 }}>
									<Link href="#" variant="body2">
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
