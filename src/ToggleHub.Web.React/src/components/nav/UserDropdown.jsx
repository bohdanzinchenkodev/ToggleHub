import React, { useState } from 'react';
import {
	Box,
	IconButton,
	Menu,
	MenuItem,
	Typography,
	Avatar,
	Divider,
	ListItemIcon,
	ListItemText
} from '@mui/material';
import {
	Person as PersonIcon,
	Logout as LogoutIcon,
	AccountCircle as AccountCircleIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router';
import { useGetUserQuery, useLogoutMutation } from '../../redux/slices/apiSlice.js';

const UserDropdown = () => {
	const [anchorEl, setAnchorEl] = useState(null);
	const navigate = useNavigate();
	const { data: user } = useGetUserQuery();
	const [logout, { isLoading: isLoggingOut }] = useLogoutMutation();

	const handleClick = (event) => {
		setAnchorEl(event.currentTarget);
	};

	const handleClose = () => {
		setAnchorEl(null);
	};

	const handleProfile = () => {
		// TODO: Navigate to profile page when it exists
		console.log('Navigate to profile');
		handleClose();
	};

	const handleLogout = async () => {
		try {
			await logout().unwrap();
			navigate('/welcome');
		} catch (error) {
			console.error('Logout failed:', error);
		}
		handleClose();
	};

	if (!user)
		return null;

	const getInitials = (name) => {
		if (!name) return 'U';
		return name
			.split(' ')
			.map(word => word.charAt(0))
			.join('')
			.toUpperCase()
			.slice(0, 2);
	};

	return (
		<Box>
			<IconButton
				onClick={handleClick}
                disableRipple
				sx={{
					display: 'flex',
					alignItems: 'center',
					gap: 1,
					color: 'inherit'
				}}
			>
				<Avatar
					sx={{
						width: 32,
						height: 32,
						bgcolor: 'primary.main',
						fontSize: '0.875rem'
					}}
				>
					{getInitials(user.userName || user.email)}
				</Avatar>
				<Typography variant="body2" sx={{ display: { xs: 'none', sm: 'block' } }}>
					{user.userName || user.email}
				</Typography>
			</IconButton>

			<Menu
				anchorEl={anchorEl}
				open={Boolean(anchorEl)}
				onClose={handleClose}
				anchorOrigin={{
					vertical: 'bottom',
					horizontal: 'right',
				}}
				transformOrigin={{
					vertical: 'top',
					horizontal: 'right',
				}}
				PaperProps={{
					sx: {
						mt: 1,
						minWidth: 200,
					}
				}}
			>
				<Box sx={{ px: 2, py: 1 }}>
					<Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
						{user.userName || 'User'}
					</Typography>
					<Typography variant="body2" color="text.secondary">
						{user.email}
					</Typography>
				</Box>

				<Divider />

				<MenuItem onClick={handleProfile}>
					<ListItemIcon>
						<PersonIcon fontSize="small" />
					</ListItemIcon>
					<ListItemText>Profile</ListItemText>
				</MenuItem>

				<MenuItem onClick={handleLogout} disabled={isLoggingOut}>
					<ListItemIcon>
						<LogoutIcon fontSize="small" />
					</ListItemIcon>
					<ListItemText>
						{isLoggingOut ? 'Signing out...' : 'Sign Out'}
					</ListItemText>
				</MenuItem>
			</Menu>
		</Box>
	);
};

export default UserDropdown;
