import React from 'react';
import { Box, Typography, CircularProgress, Alert, List } from '@mui/material';
import FlagItem from './FlagItem.jsx';

const FlagsList = ({ 
	flags, 
	isLoading, 
	isError, 
	error, 
	processingFlags, 
	onFlagToggle 
}) => {
	return (
		<Box>
			<Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
				Flags
				{isLoading && <CircularProgress size={20} />}
				{flags && flags.total > 0 && (
					<Typography variant="caption" color="text.secondary">
						({flags.total} total)
					</Typography>
				)}
			</Typography>

			{isError && (
				<Alert severity="error" sx={{ mb: 2 }}>
					Failed to load flags: {error?.data?.detail || 'Unknown error'}
				</Alert>
			)}

			{flags && flags.data && flags.data.length > 0 ? (
				<List sx={{ maxHeight: 400, overflow: 'auto' }}>
					{flags.data.map((flag) => (
						<FlagItem
							key={flag.id}
							flag={flag}
							isProcessing={processingFlags.has(flag.id)}
							onToggle={onFlagToggle}
						/>
					))}
				</List>
			) : flags && flags.data && flags.data.length === 0 ? (
				<Alert severity="info">
					No feature flags found for this environment.
				</Alert>
			) : !isLoading && !isError ? (
				<Typography variant="body2" color="text.secondary">
					Select an environment to view feature flags.
				</Typography>
			) : null}
		</Box>
	);
};

export default FlagsList;
