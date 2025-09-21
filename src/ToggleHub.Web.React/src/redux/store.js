// store.js
import { configureStore } from '@reduxjs/toolkit';
import { api } from './slices/apiSlice';
import appStateReducer from './slices/appStateSlice';
import notificationsReducer from './slices/notificationsSlice';

export const store = configureStore({
	reducer: {
		[api.reducerPath]: api.reducer,
		appState: appStateReducer,
		notifications: notificationsReducer,
	},
	middleware: (getDefaultMiddleware) =>
		getDefaultMiddleware().concat(api.middleware),
});
