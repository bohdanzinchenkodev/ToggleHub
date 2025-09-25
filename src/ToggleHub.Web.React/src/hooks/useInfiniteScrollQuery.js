import { useState, useEffect, useRef } from 'react';

const useInfiniteScrollQuery = ({
	useQuery,
	baseQueryParams = {},
	skipCondition = false
}) => {
	const [currentPage, setCurrentPage] = useState(1);
	const [allItems, setAllItems] = useState([]);
	const [hasNextPage, setHasNextPage] = useState(false);
	const [totalCount, setTotalCount] = useState(0);
	const loadingRef = useRef(null);
	const isFetchingRef = useRef(false);

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

	// Keep isFetching in sync with ref to avoid stale closures
	isFetchingRef.current = isFetching;

	// Reset pagination when baseQueryParams change (moved before data handling)
	const baseQueryParamsString = JSON.stringify(baseQueryParams);
	useEffect(() => {
		setCurrentPage(1);
		setAllItems([]);
		setHasNextPage(false);
		setTotalCount(0);
		console.log('Resetting pagination due to baseQueryParams change')
	}, [baseQueryParamsString]);

	// Handle new data coming in
	useEffect(() => {

		if (!queryData || skipCondition)
			return;

		const items = queryData.data || [];
		const hasNextPageFromResponse = queryData.hasNextPage;
		const totalFromResponse = queryData.total || 0;
		const pageFromResponse = queryData.pageIndex; // Use page from response if available

		if (pageFromResponse !== currentPage - 1)
			return;


		setAllItems(prev => [...prev, ...items]);
		console.log('Setting items:', items);
		setHasNextPage(hasNextPageFromResponse);
		setTotalCount(totalFromResponse);

	}, [queryData, skipCondition, currentPage, baseQueryParamsString]); // Add baseQueryParamsString dependency

	// Intersection Observer for infinite scroll
	useEffect(() => {
		if (!hasNextPage || isFetchingRef.current || skipCondition || !loadingRef.current) {
			return;
		}

		const observer = new IntersectionObserver(
			(entries) => {
				const entry = entries[0];
				if (entry.isIntersecting && hasNextPage && !isFetchingRef.current) {
					setCurrentPage(prev => prev + 1);
				}
			},
			{
				rootMargin: '100px',
				threshold: 0.1
			}
		);

		const currentRef = loadingRef.current;
		if (currentRef) {
			observer.observe(currentRef);
		}

		return () => {
			if (currentRef) {
				observer.unobserve(currentRef);
			}
		};
	}, [hasNextPage, skipCondition]);

	// Custom refetch that resets pagination
	const refetch = () => {
		setCurrentPage(1);
		setAllItems([]);
		setHasNextPage(false);
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
