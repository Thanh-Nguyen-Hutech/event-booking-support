import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './AuthContext'; // Nhớ import Provider để quản lý state
import AuthContainer from './components/AuthContainer'; // Import component chứa Login/Register
import ProtectedRoute from './components/ProtectedRoute'; // Import file bảo vệ route
// import Dashboard from './pages/Dashboard';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          {/* Trang Đăng nhập/Đăng ký dùng chung 1 route */}
          <Route path="/auth" element={<AuthContainer />} />
          
          {/* Các trang được bảo vệ */}
          {/* <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } 
          /> */}
          
          {/* Route mặc định hoặc trang chủ */}
          <Route path="/" element={<div>Trang chủ</div>} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;