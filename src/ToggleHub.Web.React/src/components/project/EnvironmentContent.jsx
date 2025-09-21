import React from 'react';
import { Box, Typography, Paper, Divider, Button } from '@mui/material';
import { Add as AddIcon, Key as KeyIcon } from '@mui/icons-material';
import { Link } from 'react-router';
import { getEnvironmentStyle } from '../../constants/environmentConfig.js';
import FlagsList from './FlagsList.jsx';

const EnvironmentContent = ({
	environment,
	flags,
	isFlagsLoading,
	isFlagsError,
	flagsError,
	processingFlags,
	onFlagToggle,
	orgSlug,
	projectSlug
}) => {
	if (!environment) {
		return null;
	}

	return (
		<Box sx={{ flexGrow: 1 }}>
			<Paper sx={{ p: 3, minHeight: 400 }}>
				<Box>
					<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
						<Typography variant="h5" sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
							<Box sx={getEnvironmentStyle(environment.type, 'CONTENT_CHIP')}>
								{`${environment.type} Environment`}
							</Box>
						</Typography>
						
						<Box sx={{ display: 'flex', gap: 2 }}>
							<Button
								component={Link}
								to={`/organizations/${orgSlug}/projects/${projectSlug}/environments/${environment.type}/apikeys`}
								variant="outlined"
								startIcon={<KeyIcon />}
							>
								API Keys
							</Button>
							<Button
								component={Link}
								to={`/organizations/${orgSlug}/projects/${projectSlug}/environments/${environment.type}/flags/create`}
								variant="contained"
								startIcon={<AddIcon />}
								sx={{
									backgroundColor: 'primary.main',
									'&:hover': {
										backgroundColor: 'primary.dark',
									},
								}}
							>
								Create New Flag
							</Button>
						</Box>
					</Box>

					<Divider sx={{ my: 3 }} />

					<FlagsList
						flags={flags}
						isLoading={isFlagsLoading}
						isError={isFlagsError}
						error={flagsError}
						processingFlags={processingFlags}
						onFlagToggle={onFlagToggle}
						hasSelectedEnvironment={!!environment}
						environmentType={environment.type}
					/>
				</Box>
			</Paper>
		</Box>
	);
};

export default EnvironmentContent;
