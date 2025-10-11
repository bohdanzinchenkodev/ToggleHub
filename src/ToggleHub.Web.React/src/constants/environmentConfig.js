// Environment type constants
export const ENVIRONMENT_TYPES = {
	PROD: 'Prod',
	STAGING: 'Staging',
	DEV: 'Dev'
};

// Environment color configuration
export const ENVIRONMENT_COLORS = {
	[ENVIRONMENT_TYPES.PROD]: 'error.main',
	[ENVIRONMENT_TYPES.STAGING]: 'warning.main',
	[ENVIRONMENT_TYPES.DEV]: 'success.main'
};

// Helper function to get environment color
export const getEnvironmentColor = (environmentType) => {
	return ENVIRONMENT_COLORS[environmentType] || 'primary.main';
};

// Environment styling configurations
export const ENVIRONMENT_STYLES = {
	SIDEBAR_TAB: {
		px: 3,
		py: 2,
		border: 2,
		textAlign: 'center',
		fontWeight: 'medium'
	},
	CONTENT_CHIP: {
		px: 2,
		py: 1,
		border: 2,
		textAlign: 'center',
		fontWeight: 'medium',
		fontSize: '0.875rem'
	}
};

// Helper function to get environment style with color
export const getEnvironmentStyle = (environmentType, styleType = 'SIDEBAR_TAB') => {
	const baseStyle = ENVIRONMENT_STYLES[styleType];
	const color = getEnvironmentColor(environmentType);

	return {
		...baseStyle,
		borderColor: color,
		color: color
	};
};
