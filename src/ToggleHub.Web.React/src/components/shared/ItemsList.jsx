import React from 'react';
import {
	Box,
	Typography,
	Card,
	CardContent,
	CircularProgress,
	Alert
} from "@mui/material";
import { Link } from "react-router";

const ItemsList = ({ 
	title, 
	items, 
	isLoading, 
	isError, 
	error, 
	emptyMessage, 
	getItemLink,
	onItemClick,
	renderCustomItem 
}) => {
	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 2, textAlign: 'center' }}>
					{title} ({items?.length || 0})
				</Typography>

				{isLoading ? (
					<Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
						<CircularProgress size={24} />
					</Box>
				) : isError ? (
					<Alert severity="error" sx={{ mb: 2 }}>
						{error?.data?.detail || 'Failed to load items'}
					</Alert>
				) : items?.length === 0 ? (
					<Card>
						<CardContent>
							<Typography color="text.secondary" textAlign="center" sx={{ whiteSpace: "normal", wordBreak: "break-word" }}>
								{emptyMessage}
							</Typography>
						</CardContent>
					</Card>
				) : (
					<Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, p: 2 }}>
						{items?.map((item) => 
							renderCustomItem ? (
								renderCustomItem(item)
							) : (
								<Link
									key={item.id}
									to={getItemLink(item)}
									style={{ textDecoration: 'none' }}
									onClick={() => onItemClick && onItemClick(item)}
								>
									<Typography
										variant="body1"
										sx={{
											color: 'primary.main',
											cursor: 'pointer',
											'&:hover': {
												textDecoration: 'underline'
											}
										}}
									>
										{item.name}
									</Typography>
								</Link>
							)
						)}
					</Box>
				)}
			</CardContent>
		</Card>
	);
};

export default ItemsList;
