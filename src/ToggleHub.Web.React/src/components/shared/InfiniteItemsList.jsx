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

// Default item renderer (original simple link style)
const DefaultItemRenderer = ({ item, getItemLink, onItemClick }) => (
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
);

const InfiniteItemsList = ({ 
	title, 
	items = [], 
	totalCount = 0,
	isLoading = false, 
	isError = false, 
	error, 
	emptyMessage, 
	getItemLink,
	onItemClick,
	hasNextPage = false,
	isFetchingNextPage = false,
	loadingRef,
	// New flexible rendering props
	renderItem = DefaultItemRenderer,
	containerProps = {},
	loadingMessage = "Loading more..."
}) => {
	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 2, textAlign: 'center' }}>
					{title} {totalCount > 0 ? `(${totalCount})` : `(${items.length})`}
				</Typography>

				{isLoading ? (
					<Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
						<CircularProgress size={24} />
					</Box>
				) : isError ? (
					<Alert severity="error" sx={{ mb: 2 }}>
						{error?.data?.detail || 'Failed to load items'}
					</Alert>
				) : items.length === 0 ? (
					<Card>
						<CardContent>
							<Typography color="text.secondary" textAlign="center" sx={{ whiteSpace: "normal", wordBreak: "break-word" }}>
								{emptyMessage}
							</Typography>
						</CardContent>
					</Card>
				) : (
					<Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, p: 2, ...containerProps }}>
						{items.map((item) => 
							renderItem({ 
								item, 
								getItemLink, 
								onItemClick,
								key: item.id 
							})
						)}

						{/* Infinite scroll trigger and loading indicator */}
						{hasNextPage && (
							<Box 
								ref={loadingRef}
                                id="infinite-scroll-trigger"
								sx={{ 
									display: 'flex', 
									justifyContent: 'center', 
									p: 2,
									minHeight: '40px'
								}}
							>
								{isFetchingNextPage && (
									<>
										<CircularProgress size={20} />
										<Typography variant="body2" sx={{ ml: 1, color: 'text.secondary' }}>
											{loadingMessage}
										</Typography>
									</>
								)}
							</Box>
						)}
					</Box>
				)}
			</CardContent>
		</Card>
	);
};

export default InfiniteItemsList;
