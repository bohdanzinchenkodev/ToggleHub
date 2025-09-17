import React from 'react';
import { Box, Typography, Paper, Divider } from '@mui/material';
import { getEnvironmentStyle } from '../../constants/environmentConfig.js';
import FlagsList from './FlagsList.jsx';

const EnvironmentContent = ({
	environment,
	flags,
	isFlagsLoading,
	isFlagsError,
	flagsError,
	processingFlags,
	onFlagToggle
}) => {
	if (!environment) {
		return null;
	}

	return (
		<Box sx={{ flexGrow: 1 }}>
			<Paper sx={{ p: 3, minHeight: 400 }}>
				<Box>
					<Typography variant="h5" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
						<Box sx={getEnvironmentStyle(environment.type, 'CONTENT_CHIP')}>
							{`${environment.type} Environment`}
						</Box>
					</Typography>

					<Divider sx={{ my: 3 }} />

					<FlagsList
						flags={flags}
						isLoading={isFlagsLoading}
						isError={isFlagsError}
						error={flagsError}
						processingFlags={processingFlags}
						onFlagToggle={onFlagToggle}
					/>
				</Box>
			</Paper>
		</Box>
	);
};

export default EnvironmentContent;
