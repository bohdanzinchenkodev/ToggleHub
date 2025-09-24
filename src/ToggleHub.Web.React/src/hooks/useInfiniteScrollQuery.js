import { useState, useEffect, useRef } from 'react';

const useInfiniteScrollQuery = ({
	useQuery,
	baseQueryParams = {},
	skipCondition = false
}) => {
	const [currentPage, setCurrentPage] = useState(1);
	const [allItems, setAllItems] = useState([]);
	const [hasNextPage, setHasNextPage] = useState(true);
	const loadingRef = useRef(null);
	const lastProcessedPageRef = useRef(0); // Track which page we last processed

	// Build query parameters with current page
	const queryParams = {
		...baseQueryParams,
		page: currentPage
	};

	// Use the provided RTK Query hook
	const {
		data: queryData,
		isLoading,
		isError,
		error,
		isFetching,
		refetch: originalRefetch
	} = useQuery(queryParams, { skip: skipCondition });

	// Handle new data coming in
	useEffect(() => {
		if (queryData && !skipCondition) {
			// Support different response structures
			const items = queryData.items || queryData.data || queryData || [];
			const hasNextPageFromResponse = queryData.hasNextPage;
			const pageFromResponse = queryData.page || currentPage; // Use page from response if available

			console.log('Processing data for page:', pageFromResponse, 'items:', items.length, 'lastProcessed:', lastProcessedPageRef.current);

			// Only process if this is a new page we haven't seen before
			if (pageFromResponse > lastProcessedPageRef.current) {
				lastProcessedPageRef.current = pageFromResponse;

				if (pageFromResponse === 1) {
					// First page - replace all items
					setAllItems(items);
				} else {
					// Subsequent pages - append to existing items
					setAllItems(prev => [...prev, ...items]);
				}

				setHasNextPage(hasNextPageFromResponse);
			}
		}
	}, [queryData, skipCondition]);

	// Reset state when skip condition changes or base params change significantly
	useEffect(() => {
		if (skipCondition) {
			setAllItems([]);
			setCurrentPage(1);
			setHasNextPage(true);
			lastProcessedPageRef.current = 0; // Reset processed page tracker
		}
	}, [skipCondition]);

	// Intersection Observer for infinite scroll
	useEffect(() => {
		console.log('Setting up intersection observer:', {
			hasNextPage,
			isFetching,
			skipCondition,
			loadingRefCurrent: loadingRef.current,
			allItemsLength: allItems.length
		});

		if (!hasNextPage || isFetching || skipCondition || !loadingRef.current) return;

		const observer = new IntersectionObserver(
			(entries) => {
				console.log('Intersection observer triggered:', entries[0].isIntersecting, 'isFetching:', isFetching);
				if (entries[0].isIntersecting && !isFetching) {
					console.log('Loading next page, current page:', currentPage);
					setCurrentPage(prev => prev + 1);
				}
			},
			{ rootMargin: '100px' }
		);

		const currentRef = loadingRef.current;
		if (currentRef) {
			observer.observe(currentRef);
			console.log('Observer attached to:', currentRef);
		}

		return () => {
			if (currentRef) {
				observer.unobserve(currentRef);
			}
		};
	}, [hasNextPage, isFetching, skipCondition, allItems.length]);

	// Custom refetch that resets pagination
	const refetch = () => {
		setCurrentPage(1);
		setAllItems([]);
		setHasNextPage(true);
		lastProcessedPageRef.current = 0; // Reset processed page tracker
		return originalRefetch();
	};

	return {
		allItems,
		isLoading: isLoading && currentPage === 1, // Only show loading for first page
		isError,
		error,
		hasNextPage,
		isFetchingNextPage: isFetching && currentPage > 1, // Loading indicator for subsequent pages
		loadingRef,
		refetch
	};
};

export default useInfiniteScrollQuery;
