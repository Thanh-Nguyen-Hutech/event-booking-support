import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from "../AuthContext";

export const ProtectedRoute = ({ children }) => {
  const { user, loading } = useAuth();
  
  if (loading) return <div>Đang kiểm tra...</div>;
  if (!user) return <Navigate to="/register" />;
  
  return children;
};

export default ProtectedRoute;