import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Navbar from './components/Navbar';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ServicesPage from './pages/ServicesPage';
import BookAppointmentPage from './pages/BookAppointmentPage';
import MyAppointmentsPage from './pages/MyAppointmentsPage';
import AdminDashboardPage from './pages/AdminDashboardPage';

function ProtectedRoute({ children }) {
  const { user } = useAuth();
  return user ? children : <Navigate to="/login" replace />;
}

function AdminRoute({ children }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  if (user.role !== 'Admin') return <Navigate to="/services" replace />;
  return children;
}

function Layout({ children }) {
  return (
    <div className="min-h-screen flex flex-col bg-[#FFF5F7]">
      <Navbar />
      <main className="flex-1">{children}</main>
      <footer className="bg-white border-t border-[#FFD1DC] py-10">
        <div className="text-center">
          <p className="font-display italic text-[#C96E8A] text-2xl mb-2">
            Bella Beauty Salon
          </p>
          <div className="flex items-center justify-center gap-3 mb-3">
            <div className="w-10 h-px bg-[#FFD1DC]" />
            <span className="text-[#FFB6C1] text-xs">✦</span>
            <div className="w-10 h-px bg-[#FFD1DC]" />
          </div>
          <p className="text-[#9B8590] text-xs tracking-[3px] uppercase">
            © {new Date().getFullYear()} · All Rights Reserved
          </p>
        </div>
      </footer>
    </div>
  );
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/services" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        path="/services"
        element={
          <Layout>
            <ServicesPage />
          </Layout>
        }
      />
      <Route
        path="/book"
        element={
          <ProtectedRoute>
            <Layout>
              <BookAppointmentPage />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/my-appointments"
        element={
          <ProtectedRoute>
            <Layout>
              <MyAppointmentsPage />
            </Layout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/admin"
        element={
          <AdminRoute>
            <Layout>
              <AdminDashboardPage />
            </Layout>
          </AdminRoute>
        }
      />
      <Route path="*" element={<Navigate to="/services" replace />} />
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}
