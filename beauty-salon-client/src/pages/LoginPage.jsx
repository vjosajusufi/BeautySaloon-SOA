import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const { data } = await api.post('/auth/login', form);
      login(data.token);
      navigate('/services');
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex">

      {/* Left — image panel (desktop only) */}
      <div
        className="hidden lg:flex lg:w-1/2 relative flex-col justify-end"
        style={{
          backgroundImage: 'url(https://images.unsplash.com/photo-1521590832167-7bcbfaa6381f?w=800)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        {/* Gradient overlay — darker at bottom for legibility */}
        <div className="absolute inset-0 bg-gradient-to-t from-[#2A1520]/80 via-[#2A1520]/30 to-transparent" />

        <div className="relative z-10 p-12 pb-14">
          <p className="font-display italic text-white text-3xl mb-3">
            Bella Beauty Salon
          </p>
          <div className="flex items-center gap-2 mb-4">
            <div className="w-8 h-px bg-[#FFB6C1]" />
            <span className="text-[#FFB6C1] text-xs">✦</span>
          </div>
          <p className="text-white/75 text-sm font-light leading-relaxed max-w-xs">
            Where beauty meets tranquility. Experience the finest treatments
            crafted just for you.
          </p>
        </div>
      </div>

      {/* Right — form */}
      <div className="flex-1 bg-gradient-to-b from-[#FFF0F5] to-[#FFF5F7] flex items-center justify-center px-8 py-12">
        <div className="w-full max-w-md">

          {/* Header */}
          <div className="text-center mb-8">
            <p className="text-[#FFB6C1] text-2xl mb-3">✦</p>
            <h1 className="font-display text-4xl text-[#3D2430] mb-2">
              Welcome Back
            </h1>
            <p className="font-display italic text-[#C96E8A] text-lg">
              Bella Beauty Salon
            </p>
          </div>

          {/* Card */}
          <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_8px_40px_rgba(255,182,193,0.2)] px-8 py-10">

            <div className="flex items-center gap-3 mb-8">
              <div className="flex-1 h-px bg-[#FFD1DC]" />
              <span className="text-[#C9A8B0] text-xs tracking-[3px] uppercase">Sign In</span>
              <div className="flex-1 h-px bg-[#FFD1DC]" />
            </div>

            {error && (
              <div className="bg-red-50 border border-red-200 text-red-500 rounded-xl px-4 py-3 text-sm mb-6">
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="block text-xs tracking-[2px] uppercase text-[#6B5560] mb-2 font-medium">
                  Email Address
                </label>
                <input
                  type="email"
                  name="email"
                  value={form.email}
                  onChange={handleChange}
                  required
                  placeholder="you@example.com"
                  className="w-full border border-[#FFD1DC] rounded-xl px-4 py-3 text-[#3D2430] placeholder-[#C9A8B0] focus:outline-none focus:border-[#FFB6C1] focus:ring-2 focus:ring-[#FFB6C1]/20 transition-all bg-white text-sm"
                />
              </div>

              <div>
                <label className="block text-xs tracking-[2px] uppercase text-[#6B5560] mb-2 font-medium">
                  Password
                </label>
                <input
                  type="password"
                  name="password"
                  value={form.password}
                  onChange={handleChange}
                  required
                  placeholder="••••••••"
                  className="w-full border border-[#FFD1DC] rounded-xl px-4 py-3 text-[#3D2430] placeholder-[#C9A8B0] focus:outline-none focus:border-[#FFB6C1] focus:ring-2 focus:ring-[#FFB6C1]/20 transition-all bg-white text-sm"
                />
              </div>

              <div className="pt-2">
                <button
                  type="submit"
                  disabled={loading}
                  className="w-full bg-[#C96E8A] hover:bg-[#A85471] disabled:bg-[#E5B8C8] text-white font-medium py-3 rounded-full tracking-widest uppercase text-xs transition-colors"
                >
                  {loading ? 'Signing In…' : 'Sign In'}
                </button>
              </div>
            </form>

            <p className="text-center text-[#9B8590] text-sm mt-7">
              New to Bella Beauty?{' '}
              <Link to="/register" className="text-[#C96E8A] hover:text-[#A85471] font-medium transition-colors">
                Create an account
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
