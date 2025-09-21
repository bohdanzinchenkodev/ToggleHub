import { createSlice } from '@reduxjs/toolkit';

const notificationsSlice = createSlice({
  name: 'notifications',
  initialState: {
    notifications: []
  },
  reducers: {
    addNotification: (state, action) => {
      const notification = {
        id: Date.now() + Math.random(), // Simple unique ID
        message: action.payload.message,
        severity: action.payload.severity || 'success',
        autoHideDuration: action.payload.autoHideDuration || 6000,
        timestamp: Date.now()
      };
      state.notifications.push(notification);
    },
    removeNotification: (state, action) => {
      state.notifications = state.notifications.filter(
        notification => notification.id !== action.payload
      );
    },
    clearAllNotifications: (state) => {
      state.notifications = [];
    }
  }
});

export const { addNotification, removeNotification, clearAllNotifications } = notificationsSlice.actions;

// Action creators for different notification types
export const showSuccess = (message, autoHideDuration = 6000) => 
  addNotification({ message, severity: 'success', autoHideDuration });

export const showError = (message, autoHideDuration = 6000) => 
  addNotification({ message, severity: 'error', autoHideDuration });

export const showWarning = (message, autoHideDuration = 6000) => 
  addNotification({ message, severity: 'warning', autoHideDuration });

export const showInfo = (message, autoHideDuration = 6000) => 
  addNotification({ message, severity: 'info', autoHideDuration });

// Selectors
export const selectNotifications = (state) => state.notifications.notifications;

export default notificationsSlice.reducer;
