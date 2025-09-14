import React from "react";
import Navbar from "../nav/Navbar.jsx";
import {Container} from "@mui/material";

const Layout = ({ children }) => {
	return <>
		<Navbar/>
		{children}
	</>
}
export default Layout;
