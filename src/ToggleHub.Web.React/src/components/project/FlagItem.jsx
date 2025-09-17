import React from 'react';
import { Box, Typography, Chip, Switch, ListItem } from '@mui/material';

const FlagItem = ({ flag, isProcessing, onToggle }) => {
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
				<Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
					<Chip
						label={flag.enabled ? 'Enabled' : 'Disabled'}
						color={flag.enabled ? 'success' : 'default'}
						size="small"
					/>
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
