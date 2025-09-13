import React from "react";
import {Navigate} from "react-router";
import {useGetUserQuery} from "../redux/slices/apiSlice.js";

const ProtectedRoute = ({ children }) => {
	const { data: user, isLoading } = useGetUserQuery();
	if (isLoading)
		return <p>Loading...</p>;

	if (!user)
		return <Navigate to="/welcome" replace />;

	return children;
}
export default ProtectedRoute;
