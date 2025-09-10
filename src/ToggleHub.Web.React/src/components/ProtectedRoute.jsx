import React from "react";
import {useAuth} from "../hooks/useAuth.js";
import {Navigate} from "react-router";

const ProtectedRoute = ({ children }) => {
	const {user, loading} = useAuth();
	if (loading) return <p>Loading...</p>;

	if (!user) {
		return <Navigate to="/welcome" replace />;
	}

	return children;
}
export default ProtectedRoute;
