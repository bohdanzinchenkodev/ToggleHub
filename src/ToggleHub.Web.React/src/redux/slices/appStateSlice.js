import { createSlice } from '@reduxjs/toolkit';

const initialState = {
	currentOrganization: null,
	currentProject: null,
};

const appStateSlice = createSlice({
	name: 'appState',
	initialState,
	reducers: {
		setCurrentOrganization: (state, action) => {
			state.currentOrganization = action.payload;
			// Clear current project when organization changes
			if (state.currentProject && state.currentProject.organizationId !== action.payload?.id) {
				state.currentProject = null;
			}
		},
		setCurrentProject: (state, action) => {
			state.currentProject = action.payload;
		},
		clearCurrentOrganization: (state) => {
			state.currentOrganization = null;
			state.currentProject = null; // Clear project when organization is cleared
		},
		clearCurrentProject: (state) => {
			state.currentProject = null;
		},
		clearAppState: (state) => {
			state.currentOrganization = null;
			state.currentProject = null;
		},
	},
});

export const {
	setCurrentOrganization,
	setCurrentProject,
	clearCurrentOrganization,
	clearCurrentProject,
	clearAppState,
} = appStateSlice.actions;

// Selectors
export const selectCurrentOrganization = (state) => state.appState.currentOrganization;
export const selectCurrentProject = (state) => state.appState.currentProject;

export default appStateSlice.reducer;
