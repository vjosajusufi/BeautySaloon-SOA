import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '' });
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
      const { data } = await api.post('/auth/register', form);
      login(data.token);
      navigate('/services');
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  const inputClass =
    'w-full border border-[#FFD1DC] rounded-xl px-4 py-3 text-[#3D2430] placeholder-[#C9A8B0] focus:outline-none focus:border-[#FFB6C1] focus:ring-2 focus:ring-[#FFB6C1]/20 transition-all bg-white text-sm';
  const labelClass =
    'block text-xs tracking-[2px] uppercase text-[#6B5560] mb-2 font-medium';

  return (
    <div className="min-h-screen bg-gradient-to-b from-[#FFF0F5] to-[#FFF5F7] flex items-center justify-center px-4 py-12">
      <div className="w-full max-w-md">

        {/* Header */}
        <div className="text-center mb-8">
          <p className="text-[#FFB6C1] text-2xl mb-3">✦</p>
          <h1 className="font-display text-4xl text-[#3D2430] mb-2">
            Join Us
          </h1>
          <p className="font-display italic text-[#C96E8A] text-lg">
            Begin your beauty journey
          </p>
        </div>

        {/* Card */}
        <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_8px_40px_rgba(255,182,193,0.2)] px-8 py-10">

          <div className="flex items-center gap-3 mb-8">
            <div className="flex-1 h-px bg-[#FFD1DC]" />
            <span className="text-[#C9A8B0] text-xs tracking-[3px] uppercase">Create Account</span>
            <div className="flex-1 h-px bg-[#FFD1DC]" />
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-500 rounded-xl px-4 py-3 text-sm mb-6">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className={labelClass}>First Name</label>
                <input
                  type="text"
                  name="firstName"
                  value={form.firstName}
                  onChange={handleChange}
                  required
                  placeholder="Jane"
                  className={inputClass}
                />
              </div>
              <div>
                <label className={labelClass}>Last Name</label>
                <input
                  type="text"
                  name="lastName"
                  value={form.lastName}
                  onChange={handleChange}
                  required
                  placeholder="Doe"
                  className={inputClass}
                />
              </div>
            </div>

            <div>
              <label className={labelClass}>Email Address</label>
              <input
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                required
                placeholder="you@example.com"
                className={inputClass}
              />
            </div>

            <div>
              <label className={labelClass}>Password</label>
              <input
                type="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                required
                placeholder="••••••••"
                className={inputClass}
              />
            </div>

            <div className="pt-2">
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#C96E8A] hover:bg-[#A85471] disabled:bg-[#E5B8C8] text-white font-medium py-3 rounded-full tracking-widest uppercase text-xs transition-colors"
              >
                {loading ? 'Creating Account…' : 'Create Account'}
              </button>
            </div>
          </form>

          <p className="text-center text-[#9B8590] text-sm mt-7">
            Already have an account?{' '}
            <Link to="/login" className="text-[#C96E8A] hover:text-[#A85471] font-medium transition-colors">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
