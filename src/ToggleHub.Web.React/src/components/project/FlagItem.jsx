import React from 'react';
import { Box, Typography, Chip, Switch, ListItem, IconButton, Tooltip } from '@mui/material';
import { Edit as EditIcon } from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router';

const FlagItem = ({ flag, isProcessing, onToggle, environmentType }) => {
	const navigate = useNavigate();
	const { orgSlug, projectSlug } = useParams();

	const handleEdit = () => {
		navigate(`/organizations/${orgSlug}/projects/${projectSlug}/environments/${environmentType}/flags/${flag.id}/edit`);
	};

	return (
		<ListItem 
			sx={{
				py: 2,
				px: 0,
				borderBottom: 1,
				borderColor: 'divider',
				'&:last-child': { borderBottom: 0 }
			}}
		>
			<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', width: '100%' }}>
				<Box sx={{ flex: 1 }}>
					<Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
						{flag.key}
					</Typography>
					<Typography variant="caption" color="text.secondary">
						ID: {flag.id}
					</Typography>
				</Box>
				<Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
					<Chip
						label={flag.enabled ? 'Enabled' : 'Disabled'}
						color={flag.enabled ? 'success' : 'default'}
						size="small"
					/>
					<Tooltip title="Edit flag">
						<IconButton 
							size="small" 
							onClick={handleEdit}
							color="primary"
						>
							<EditIcon fontSize="small" />
						</IconButton>
					</Tooltip>
					<Switch
						checked={flag.enabled}
						disabled={isProcessing}
						onChange={(event) => onToggle(flag, event.target.checked)}
					/>
				</Box>
			</Box>
		</ListItem>
	);
};

export default FlagItem;
