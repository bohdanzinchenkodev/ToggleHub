import React from 'react';
import { Box, Paper, Tabs, Tab, useTheme, useMediaQuery } from '@mui/material';
import { getEnvironmentStyle } from '../../constants/environmentConfig.js';

const EnvironmentTabs = ({ environments, selectedTab, onTabChange }) => {
	const theme = useTheme();
	const isMobile = useMediaQuery(theme.breakpoints.down('md'));

	if (!environments || environments.length === 0) {
		return null;
	}

	return (
		<Paper sx={{ 
			width: { xs: '100%', md: 240 }, 
			mr: { xs: 0, md: 2 },
			mb: { xs: 2, md: 0 }
		}}>
			<Tabs
				orientation={isMobile ? 'horizontal' : 'vertical'}
				variant="scrollable"
				value={selectedTab}
				onChange={onTabChange}
				sx={{ 
					minHeight: { xs: 'auto', md: 400 }
				}}
			>
				{environments.map((environment, index) => (
					<Tab
						key={environment.id}
						label={
							<Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', width: '100%' }}>
								<Box sx={{
									...getEnvironmentStyle(environment.type, 'SIDEBAR_TAB'),
									minWidth: { xs: 100, md: 200 },
									px: { xs: 2, md: 3 }
								}}>
									{environment.type}
								</Box>
							</Box>
						}
						sx={{
							alignItems: 'center',
							textAlign: 'center',
							minHeight: { xs: 48, md: 60 },
							py: { xs: 1, md: 2 },
							justifyContent: 'center',
							minWidth: { xs: 120, md: 'auto' }
						}}
					/>
				))}
			</Tabs>
		</Paper>
	);
};

export default EnvironmentTabs;
