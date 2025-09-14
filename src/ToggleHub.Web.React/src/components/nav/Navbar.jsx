import React from "react";
import { AppBar, Toolbar, Box, Typography } from "@mui/material";
import { useNavigate } from "react-router";
import logo from "../../assets/th-logo.png";

const Navbar = () => {
	const navigate = useNavigate();

	return (
		<AppBar position="static" sx={{ backgroundColor: 'transparent', boxShadow: 'none' }}>
			<Toolbar>
				<Box
					component="img"
					src={logo}
					alt="ToggleHub Logo"
					sx={{
						height: 70,
						width: 'auto',
						cursor: 'pointer',
					}}
					onClick={() => navigate('/')}
				/>
				<Typography
					variant="h4"
					component="div"
					sx={{
						fontWeight: 'bold',
						cursor: 'pointer',
						color: 'inherit'
					}}
					onClick={() => navigate('/')}
				>
					ToggleHub
				</Typography>
			</Toolbar>
		</AppBar>
	);
};

export default Navbar;
