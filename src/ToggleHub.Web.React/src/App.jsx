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

function App() {

  return (
    <Routes>
        <Route index element={
            <ProtectedRoute>
                <Home />
            </ProtectedRoute>
        } />
        <Route path="welcome" element={<Welcome />} />
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
    </Routes>
  )
}

export default App
