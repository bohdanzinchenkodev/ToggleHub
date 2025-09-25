import { useState, useEffect, useRef } from 'react';

const useInfiniteScrollQuery = ({
	useQuery,
	baseQueryParams = {},
	skipCondition = false
}) => {
	const [currentPage, setCurrentPage] = useState(1);
	const [allItems, setAllItems] = useState([]);
	const [hasNextPage, setHasNextPage] = useState(true);
	const [totalCount, setTotalCount] = useState(0);
	const loadingRef = useRef(null);

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
		if (!queryData || skipCondition)
			return;

		const items = queryData.data || [];
		const hasNextPageFromResponse = queryData.hasNextPage;
		const totalFromResponse = queryData.total || 0;
		const pageFromResponse = queryData.pageIndex; // Use page from response if available

		console.log('Processing data for page:', pageFromResponse, 'items:', items.length, 'total:', totalFromResponse);

		if (pageFromResponse !== currentPage - 1)
			return;

		if (currentPage === 1) {
			// First page - replace all items
			setAllItems(items);
		} else {
			// Subsequent pages - append to existing items
			setAllItems(prev => [...prev, ...items]);
		}

		setHasNextPage(hasNextPageFromResponse);
		setTotalCount(totalFromResponse);

	}, [queryData, skipCondition, currentPage]);

	// Reset pagination when baseQueryParams change (e.g., environment change)
	useEffect(() => {
		setCurrentPage(1);
		setAllItems([]);
		setHasNextPage(true);
		setTotalCount(0);
	}, [JSON.stringify(baseQueryParams)]);

	// Intersection Observer for infinite scroll
	useEffect(() => {
		console.log('Setting up intersection observer:', {
			hasNextPage,
			isFetching,
			skipCondition,
			loadingRefCurrent: loadingRef.current,
			allItemsLength: allItems.length
		});

		if (!hasNextPage || isFetching || skipCondition || !loadingRef.current)
			return;

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
		setTotalCount(0);
		return originalRefetch();
	};

	return {
		allItems,
		totalCount,
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
