import React from 'react';
import { Box, Typography, CircularProgress, Alert, List } from '@mui/material';
import FlagItem from './FlagItem.jsx';

const FlagsList = ({
	flags = [], // Now an array instead of object
	totalCount = 0,
	isLoading,
	isError,
	error,
	processingFlags,
	onFlagToggle,
	hasSelectedEnvironment = false,
	environmentType,
	hasNextPage = false,
	isFetchingNextPage = false,
	loadingRef
}) => {
	// Show loading if explicitly loading OR if we have a selected environment but no flags yet
	const shouldShowLoading = isLoading || (hasSelectedEnvironment && !flags && !isError);

	return (
		<Box>
		<Typography variant="h6" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
			Flags
			{totalCount > 0 && (
				<Typography variant="caption" color="text.secondary">
					({totalCount})
				</Typography>
			)}
		</Typography>
			{shouldShowLoading && (
				<Box sx={{
					display: 'flex',
					justifyContent: 'center',
					alignItems: 'center',
					minHeight: 200
				}}>
					<CircularProgress />
				</Box>
			)}
			{isError && (
				<Alert severity="error" sx={{ mb: 2 }}>
					Failed to load flags: {error?.data?.detail || 'Unknown error'}
				</Alert>
			)}
			{
				!isLoading && !isFetchingNextPage && flags && flags.length === 0 && (
					<Alert severity="info" sx={{ mb: 2 }}>
						No feature flags found for this environment.
					</Alert>
				)

			}


			{/* Always render the list and loading element */}
			<List sx={{ maxHeight: 400, overflow: 'auto' }}>
				{flags && flags.map((flag) => (
					<FlagItem
						key={flag.id}
						flag={flag}
						isProcessing={processingFlags.has(flag.id)}
						onToggle={onFlagToggle}
						environmentType={environmentType}
					/>)
				)}
				<Box
					ref={loadingRef}
					sx={{
						display: hasNextPage ? 'flex' : 'none',
						justifyContent: 'center',
						p: 2,
						minHeight: '40px'
					}}
				>
					{isFetchingNextPage && (
						<>
							<CircularProgress size={20} />
							<Typography variant="body2" sx={{ ml: 1, color: 'text.secondary' }}>
								Loading more flags...
							</Typography>
						</>
					)}
				</Box>
			</List>
		</Box>
	);
};

export default FlagsList;
