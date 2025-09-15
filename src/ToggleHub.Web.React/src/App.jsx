import './App.css'
import Welcome from "./pages/Welcome/Welcome.jsx";
import {
    Route,
    Routes
} from "react-router";
import Home from "./pages/Home.jsx";
import ProtectedRoute from "./components/ProtectedRoute.jsx";
import Login from "./pages/Login.jsx";
import Register from "./pages/Register.jsx";
import Layout from "./components/layouts/Layout.jsx";
import OrganizationsList from "./pages/OrganizationsList.jsx";
import Organization from "./pages/Organization.jsx";

function App() {

  return (
    <Routes>
        <Route index element={
            <ProtectedRoute>
                <Layout>
                    <OrganizationsList />
                </Layout>
            </ProtectedRoute>
        } />
        <Route path="organizations/:slug" element={
            <ProtectedRoute>
                <Layout>
                    <Organization />
                </Layout>
            </ProtectedRoute>
        } />
        <Route path="welcome" element={
            <Layout>
                <Welcome />
            </Layout>
        } />
        <Route path="login" element={<Layout>
            <Login />
        </Layout>} />
        <Route path="register" element={<Layout>
            <Register />
        </Layout>} />
    </Routes>
  )
}

export default App
