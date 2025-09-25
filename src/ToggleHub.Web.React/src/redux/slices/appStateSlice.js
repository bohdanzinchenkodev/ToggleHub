import { createSlice } from '@reduxjs/toolkit';

const initialState = {
	currentOrganization: null,
	currentProject: null,
	currentUserPermissions: null,
};

const appStateSlice = createSlice({
	name: 'appState',
	initialState,
	reducers: {
		setCurrentOrganization: (state, action) => {
			state.currentOrganization = action.payload;
			// Clear current project and permissions when organization changes
			if (state.currentProject && state.currentProject.organizationId !== action.payload?.id) {
				state.currentProject = null;
			}
			// Clear permissions when organization changes
			state.currentUserPermissions = null;
		},
		setCurrentProject: (state, action) => {
			state.currentProject = action.payload;
		},
		setCurrentUserPermissions: (state, action) => {
			state.currentUserPermissions = action.payload;
		},
		clearCurrentOrganization: (state) => {
			state.currentOrganization = null;
			state.currentProject = null; // Clear project when organization is cleared
			state.currentUserPermissions = null; // Clear permissions when organization is cleared
		},
		clearCurrentProject: (state) => {
			state.currentProject = null;
		},
		clearCurrentUserPermissions: (state) => {
			state.currentUserPermissions = null;
		},
		clearAppState: (state) => {
			state.currentOrganization = null;
			state.currentProject = null;
			state.currentUserPermissions = null;
		},
	},
});

export const {
	setCurrentOrganization,
	setCurrentProject,
	setCurrentUserPermissions,
	clearCurrentOrganization,
	clearCurrentProject,
	clearCurrentUserPermissions,
	clearAppState,
} = appStateSlice.actions;

// Selectors
export const selectCurrentOrganization = (state) => state.appState.currentOrganization;
export const selectCurrentProject = (state) => state.appState.currentProject;
export const selectCurrentUserPermissions = (state) => state.appState.currentUserPermissions;

export default appStateSlice.reducer;
