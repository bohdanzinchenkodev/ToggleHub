import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Snackbar, Alert } from '@mui/material';
import { selectNotifications, removeNotification } from '../../redux/slices/notificationsSlice';

const NotificationContainer = () => {
  const notifications = useSelector(selectNotifications);
  const dispatch = useDispatch();

  // Auto-remove notifications after their duration
  useEffect(() => {
    notifications.forEach(notification => {
      if (notification.autoHideDuration > 0) {
        const timer = setTimeout(() => {
          dispatch(removeNotification(notification.id));
        }, notification.autoHideDuration);

        return () => clearTimeout(timer);
      }
    });
  }, [notifications, dispatch]);

  const handleClose = (notificationId) => {
    dispatch(removeNotification(notificationId));
  };

  return (
    <>
      {notifications.map((notification, index) => (
        <Snackbar
          key={notification.id}
          open={true}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          sx={{
            // Stack notifications vertically
            top: `${80 + index * 70}px !important`,
          }}
        >
          <Alert
            onClose={() => handleClose(notification.id)}
            severity={notification.severity}
            variant="filled"
            sx={{ width: '100%' }}
          >
            {notification.message}
          </Alert>
        </Snackbar>
      ))}
    </>
  );
};

export default NotificationContainer;
