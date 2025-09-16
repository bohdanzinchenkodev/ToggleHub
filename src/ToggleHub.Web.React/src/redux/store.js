// store.js
import { configureStore } from '@reduxjs/toolkit';
import { api } from './slices/apiSlice';
import appStateReducer from './slices/appStateSlice';

export const store = configureStore({
	reducer: {
		[api.reducerPath]: api.reducer,
		appState: appStateReducer,
	},
	middleware: (getDefaultMiddleware) =>
		getDefaultMiddleware().concat(api.middleware),
});
