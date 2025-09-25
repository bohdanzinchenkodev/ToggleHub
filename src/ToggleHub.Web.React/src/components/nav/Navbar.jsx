import React from "react";
import { AppBar, Toolbar, Box, Typography } from "@mui/material";
import { Link } from "react-router";
import logo from "../../assets/th-logo.png";
import UserDropdown from "./UserDropdown.jsx";

const Navbar = () => {
	return (
		<AppBar position="static" sx={{ backgroundColor: 'transparent', boxShadow: 'none' }}>
			<Toolbar>
				<Box
					component={Link}
					to="/"
					sx={{
						height: 70,
						width: 'auto',
						cursor: 'pointer',
						display: 'block'
					}}
				>
					<Box
						component="img"
						src={logo}
						alt="ToggleHub Logo"
						sx={{
							height: 70,
							width: 'auto',
							display: 'block'
						}}
					/>
				</Box>
				<Typography
					variant="h4"
					component={Link}
					to="/"
					sx={{
						fontWeight: 'bold',
						cursor: 'pointer',
						color: 'inherit',
						textDecoration: 'none',
						flexGrow: 1
					}}
				>
					ToggleHub
				</Typography>
				
				<UserDropdown />
			</Toolbar>
		</AppBar>
	);
};

export default Navbar;
