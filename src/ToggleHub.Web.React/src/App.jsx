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
import Project from "./pages/Project.jsx";
import CreateFlag from "./pages/CreateFlag.jsx";
import UpdateFlag from "./pages/UpdateFlag.jsx";
import NotificationContainer from "./components/notifications/NotificationContainer.jsx";

function App() {

  return (
    <>
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
        <Route path="organizations/:orgSlug/projects/:projectSlug" element={
            <ProtectedRoute>
                <Layout>
                    <Project />
                </Layout>
            </ProtectedRoute>
        } />
        <Route path="organizations/:orgSlug/projects/:projectSlug/environments/:envType/flags/create" element={
            <ProtectedRoute>
                <Layout>
                    <CreateFlag />
                </Layout>
            </ProtectedRoute>
        } />
        <Route path="organizations/:orgSlug/projects/:projectSlug/environments/:envType/flags/:flagId/edit" element={
            <ProtectedRoute>
                <Layout>
                    <UpdateFlag />
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
    <NotificationContainer />
    </>
  )
}

export default App
