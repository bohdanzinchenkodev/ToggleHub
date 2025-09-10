import { useState } from 'react'
import './App.css'
import Welcome from "./pages/Welcome/Welcome.jsx";
import {
    Route,
    Routes
} from "react-router";
import Home from "./pages/Home.jsx";
import ProtectedRoute from "./components/ProtectedRoute.jsx";

function App() {
  const [count, setCount] = useState(0)

  return (
    <Routes>
        <Route index element={
            <ProtectedRoute>
                <Home />
            </ProtectedRoute>
        } />
        <Route path="Welcome" element={<Welcome />} />

    </Routes>
  )
}

export default App
