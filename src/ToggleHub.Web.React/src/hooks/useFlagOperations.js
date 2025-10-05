import { useState, useEffect, useCallback } from 'react';
import { useEnableFlagMutation, useDisableFlagMutation } from '../redux/slices/apiSlice.js';

export const useFlagOperations = (organization, project, selectedEnvironment) => {
	const [processingFlags, setProcessingFlags] = useState(new Set());
	const [localFlags, setLocalFlags] = useState(null);
	const [enableFlag] = useEnableFlagMutation();
	const [disableFlag] = useDisableFlagMutation();

	// Reset local flags when environment changes
	useEffect(() => {
		setLocalFlags(null);
		setProcessingFlags(new Set());
	}, [selectedEnvironment?.id]);

	// Sync API flags to local state
	const syncFlags = useCallback((flags) => {
		console.log('Syncing flags:', flags);
		if (flags) {
			setLocalFlags(flags);
		}
	}, []);

	// Handle flag toggle
	const handleFlagToggle = async (flag, newEnabled) => {
		console.log('Toggle attempt:', { flagId: flag.id, flagKey: flag.key, newEnabled, currentEnabled: flag.enabled });
		console.log(localFlags);

		// Immediately update local state for instant UI feedback
		setLocalFlags(prev => {
			if (!prev) return prev;
			return prev.map(f =>
				f.id === flag.id ? { ...f, enabled: newEnabled } : f
			);
		});

		// Add flag to processing set
		setProcessingFlags(prev => new Set(prev).add(flag.id));

		try {
			const mutationParams = {
				organizationId: organization?.id,
				projectId: project?.id,
				environmentId: selectedEnvironment?.id,
				flagId: flag.id
			};

			console.log('Mutation params:', mutationParams);

			if (newEnabled) {
				console.log('Calling enableFlag');
				await enableFlag(mutationParams).unwrap();
			} else {
				console.log('Calling disableFlag');
				await disableFlag(mutationParams).unwrap();
			}
			console.log('Flag toggle successful');
		} catch (error) {
			console.error('Failed to toggle flag:', error);
			// Revert the local state change on error
			setLocalFlags(prev => {
				if (!prev) return prev;
				return prev.map(f =>
					f.id === flag.id ? { ...f, enabled: !newEnabled } : f
				);
			});
		} finally {
			// Remove flag from processing set
			setProcessingFlags(prev => {
				const newSet = new Set(prev);
				newSet.delete(flag.id);
				return newSet;
			});
		}
	};

	return {
		localFlags,
		processingFlags,
		handleFlagToggle,
		syncFlags
	};
};
