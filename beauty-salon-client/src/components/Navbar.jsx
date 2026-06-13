import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <nav className="bg-white border-b border-[#FFD1DC] shadow-[0_2px_20px_rgba(255,182,193,0.12)]">
      <div className="max-w-7xl mx-auto px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">

          <Link to="/" className="flex items-center gap-2">
            <span className="text-[#FFB6C1] text-lg leading-none">✦</span>
            <span className="font-display italic text-[#3D2430] text-xl tracking-wide">
              Bella Beauty Salon
            </span>
          </Link>

          <div className="flex items-center gap-6">
            <Link
              to="/services"
              className="text-[#6B5560] hover:text-[#C96E8A] text-xs tracking-[2px] uppercase transition-colors font-medium"
            >
              Services
            </Link>

            {user ? (
              <>
                <Link
                  to="/book"
                  className="text-[#6B5560] hover:text-[#C96E8A] text-xs tracking-[2px] uppercase transition-colors font-medium"
                >
                  Book
                </Link>
                <Link
                  to="/my-appointments"
                  className="text-[#6B5560] hover:text-[#C96E8A] text-xs tracking-[2px] uppercase transition-colors font-medium"
                >
                  My Appointments
                </Link>
                {user.role === 'Admin' && (
                  <Link
                    to="/admin"
                    className="text-[#6B5560] hover:text-[#C96E8A] text-xs tracking-[2px] uppercase transition-colors font-medium"
                  >
                    Admin
                  </Link>
                )}
                <div className="flex items-center gap-3 border-l border-[#FFD1DC] pl-6">
                  <span className="text-[#9B8590] text-xs hidden sm:block truncate max-w-[140px]">
                    {user.email}
                  </span>
                  <button
                    onClick={handleLogout}
                    className="border border-[#FFB6C1] text-[#C96E8A] hover:bg-[#FFF5F7] px-4 py-1.5 rounded-full text-xs tracking-[2px] uppercase transition-colors font-medium"
                  >
                    Logout
                  </button>
                </div>
              </>
            ) : (
              <div className="flex items-center gap-3">
                <Link
                  to="/login"
                  className="text-[#6B5560] hover:text-[#C96E8A] text-xs tracking-[2px] uppercase transition-colors font-medium"
                >
                  Sign In
                </Link>
                <Link
                  to="/register"
                  className="bg-[#C96E8A] hover:bg-[#A85471] text-white px-5 py-2 rounded-full text-xs tracking-[2px] uppercase transition-colors font-medium"
                >
                  Register
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
