import React from 'react';
import {
	Box,
	Typography,
	Breadcrumbs,
	Chip
} from "@mui/material";
import {
	Business as BusinessIcon,
	Folder as ProjectIcon
} from "@mui/icons-material";
import { Link } from "react-router";
import { useAppState } from "../../hooks/useAppState.js";

const AppStateDisplay = ({ showBreadcrumbs = true, showChips = false }) => {
	const { currentOrganization, currentProject } = useAppState();

	if (!currentOrganization && !currentProject) {
		return null;
	}

	if (showBreadcrumbs) {
		return (
			<Box sx={{ mb: 2 }}>
				<Breadcrumbs>
					{currentOrganization && (
						<Link
							to={`/organizations/${currentOrganization.slug}`}
							style={{ textDecoration: 'none' }}
						>
							<Typography
								variant="body2"
								sx={{
									color: 'primary.main',
									display: 'flex',
									alignItems: 'center',
									gap: 0.5,
									'&:hover': {
										textDecoration: 'underline'
									}
								}}
							>
								<BusinessIcon fontSize="small" />
								{currentOrganization.name}
							</Typography>
						</Link>
					)}
					{currentProject && (
						<Typography
							variant="body2"
							sx={{
								color: 'text.primary',
								display: 'flex',
								alignItems: 'center',
								gap: 0.5
							}}
						>
							<ProjectIcon fontSize="small" />
							{currentProject.name}
						</Typography>
					)}
				</Breadcrumbs>
			</Box>
		);
	}

	if (showChips) {
		return (
			<Box sx={{ mb: 2, display: 'flex', gap: 1, flexWrap: 'wrap' }}>
				{currentOrganization && (
					<Chip
						icon={<BusinessIcon />}
						label={currentOrganization.name}
						variant="outlined"
						size="small"
						component={Link}
						to={`/organizations/${currentOrganization.slug}`}
						clickable
					/>
				)}
				{currentProject && (
					<Chip
						icon={<ProjectIcon />}
						label={currentProject.name}
						variant="outlined"
						size="small"
					/>
				)}
			</Box>
		);
	}

	return null;
};

export default AppStateDisplay;
