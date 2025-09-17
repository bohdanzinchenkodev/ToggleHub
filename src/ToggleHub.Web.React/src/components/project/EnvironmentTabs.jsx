import React from 'react';
import { Box, Paper, Tabs, Tab } from '@mui/material';
import { getEnvironmentStyle } from '../../constants/environmentConfig.js';

const EnvironmentTabs = ({ environments, selectedTab, onTabChange }) => {
	if (!environments || environments.length === 0) {
		return null;
	}

	return (
		<Paper sx={{ width: 240, mr: 2 }}>
			<Tabs
				orientation="vertical"
				variant="scrollable"
				value={selectedTab}
				onChange={onTabChange}
				sx={{ borderRight: 1, borderColor: 'divider', minHeight: 400 }}
			>
				{environments.map((environment, index) => (
					<Tab
						key={environment.id}
						label={
							<Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', width: '100%' }}>
								<Box sx={getEnvironmentStyle(environment.type, 'SIDEBAR_TAB')}>
									{environment.type}
								</Box>
							</Box>
						}
						sx={{
							alignItems: 'center',
							textAlign: 'center',
							minHeight: 60,
							py: 2,
							justifyContent: 'center'
						}}
					/>
				))}
			</Tabs>
		</Paper>
	);
};

export default EnvironmentTabs;
