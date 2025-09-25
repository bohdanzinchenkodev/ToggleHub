import React from 'react';
import {
	Dialog,
	DialogTitle,
	DialogContent,
	DialogContentText,
	DialogActions,
	Button
} from '@mui/material';

const ConfirmDialog = ({ open, title, content, onCancel, onConfirm, confirmText = 'Confirm', cancelText = 'Cancel', confirmColor = 'error' }) => {
	return (
		<Dialog open={open} onClose={onCancel}>
			<DialogTitle>{title}</DialogTitle>
			<DialogContent>
				<DialogContentText>
					{content}
				</DialogContentText>
			</DialogContent>
			<DialogActions>
				<Button onClick={onCancel}>{cancelText}</Button>
				<Button color={confirmColor} onClick={onConfirm} autoFocus>
					{confirmText}
				</Button>
			</DialogActions>
		</Dialog>
	);
};

export default ConfirmDialog;
