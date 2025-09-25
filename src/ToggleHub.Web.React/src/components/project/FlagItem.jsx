import React from 'react';
import { Box, Typography, Chip, Switch, ListItem, IconButton, Tooltip } from '@mui/material';
import { Edit as EditIcon } from '@mui/icons-material';
import { useParams, Link } from 'react-router';
import { usePermissions } from '../../hooks/usePermissions';
import { PERMISSIONS } from '../../constants/permissions';

const FlagItem = ({ flag, isProcessing, onToggle, environmentType }) => {
     const { orgSlug, projectSlug } = useParams();
     const { hasPermission } = usePermissions();
 
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
				</Box>
				<Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
					<Chip
						label={flag.enabled ? 'Enabled' : 'Disabled'}
						color={flag.enabled ? 'success' : 'default'}
						size="small"
					/>
					{hasPermission(PERMISSIONS.MANAGE_FLAGS) && (
						<Tooltip title="Edit flag">
							<IconButton 
								size="small" 
								component={Link}
								to={`/organizations/${orgSlug}/projects/${projectSlug}/environments/${environmentType}/flags/${flag.id}/edit`}
								color="primary"
							>
								<EditIcon fontSize="small" />
							</IconButton>
						</Tooltip>
					)}
					<Switch
						checked={flag.enabled}
						disabled={isProcessing || !hasPermission(PERMISSIONS.MANAGE_FLAGS)}
						onChange={(event) => onToggle(flag, event.target.checked)}
					/>
				</Box>
			</Box>
		</ListItem>
	);
};

export default FlagItem;
