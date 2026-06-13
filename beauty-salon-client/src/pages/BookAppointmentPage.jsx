import { useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';

const inputClass =
  'w-full border border-[#FFD1DC] rounded-xl px-4 py-3 text-[#3D2430] placeholder-[#C9A8B0] focus:outline-none focus:border-[#FFB6C1] focus:ring-2 focus:ring-[#FFB6C1]/20 transition-all bg-white text-sm';
const labelClass =
  'block text-xs tracking-[2px] uppercase text-[#6B5560] mb-2 font-medium';

export default function BookAppointmentPage() {
  const { user } = useAuth();
  const [searchParams] = useSearchParams();

  const [services, setServices] = useState([]);
  const [form, setForm] = useState({
    serviceId: searchParams.get('serviceId') || '',
    appointmentDate: '',
    startTime: '',
    notes: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    api.get('/services').then(({ data }) =>
      setServices(data.filter((s) => s.isActive))
    );
  }, []);

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');

    const userId = user?.id;
    if (!userId || isNaN(userId)) {
      setError('Session error: user ID not found. Please log out and log in again.');
      return;
    }

    setLoading(true);
    const payload = {
      userId,
      serviceId: parseInt(form.serviceId, 10),
      appointmentDate: form.appointmentDate,
      startTime: form.startTime + ':00',
      notes: form.notes || null,
    };

    try {
      await api.post('/appointments', payload);
      setSuccess(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to book appointment. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  if (success) {
    return (
      <div className="min-h-screen bg-[#FFF5F7] flex items-center justify-center px-4">
        <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_8px_40px_rgba(255,182,193,0.2)] p-12 text-center max-w-md w-full">
          <div className="w-16 h-16 bg-[#FFF5F7] border border-[#FFD1DC] rounded-full flex items-center justify-center mx-auto mb-6">
            <span className="text-[#C96E8A] text-2xl">✓</span>
          </div>
          <h2 className="font-display text-3xl text-[#3D2430] mb-2 italic">
            All Set!
          </h2>
          <p className="font-display italic text-[#C96E8A] mb-4">
            Your appointment is confirmed
          </p>
          <div className="flex items-center justify-center gap-3 mb-6">
            <div className="w-8 h-px bg-[#FFD1DC]" />
            <span className="text-[#FFB6C1] text-xs">✦</span>
            <div className="w-8 h-px bg-[#FFD1DC]" />
          </div>
          <p className="text-[#6B5560] text-sm font-light mb-8">
            We look forward to welcoming you. A confirmation has been noted for your appointment.
          </p>
          <div className="flex gap-3 justify-center">
            <button
              onClick={() => {
                setSuccess(false);
                setForm({ serviceId: '', appointmentDate: '', startTime: '', notes: '' });
              }}
              className="border border-[#FFB6C1] text-[#C96E8A] hover:bg-[#FFF5F7] px-6 py-2.5 rounded-full text-xs tracking-widest uppercase transition-colors font-medium"
            >
              Book Again
            </button>
            <Link
              to="/my-appointments"
              className="bg-[#C96E8A] hover:bg-[#A85471] text-white px-6 py-2.5 rounded-full text-xs tracking-widest uppercase transition-colors font-medium"
            >
              My Appointments
            </Link>
          </div>
        </div>
      </div>
    );
  }

  const today = new Date().toISOString().split('T')[0];

  return (
    <div className="min-h-screen bg-[#FFF5F7] py-16 px-4">
      <div className="max-w-lg mx-auto">

        {/* Header */}
        <div className="text-center mb-10">
          <p className="text-[#C96E8A] text-xs tracking-[4px] uppercase mb-3 font-medium">
            Reserve Your Visit
          </p>
          <h1 className="font-display text-4xl text-[#3D2430] mb-2">
            Book an Appointment
          </h1>
          <div className="flex items-center justify-center gap-3 mt-4">
            <div className="w-10 h-px bg-[#FFD1DC]" />
            <span className="text-[#FFB6C1] text-xs">✦</span>
            <div className="w-10 h-px bg-[#FFD1DC]" />
          </div>
        </div>

        {/* Form Card */}
        <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_8px_40px_rgba(255,182,193,0.2)] px-8 py-10">

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-500 rounded-xl px-4 py-3 text-sm mb-6">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className={labelClass}>Service</label>
              <select
                name="serviceId"
                value={form.serviceId}
                onChange={handleChange}
                required
                className={inputClass + ' appearance-none cursor-pointer'}
              >
                <option value="">Select a treatment…</option>
                {services.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.name} — {s.durationMinutes} min — ${s.price.toFixed(2)}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className={labelClass}>Preferred Date</label>
              <input
                type="date"
                name="appointmentDate"
                value={form.appointmentDate}
                onChange={handleChange}
                required
                min={today}
                className={inputClass}
              />
            </div>

            <div>
              <label className={labelClass}>Preferred Time</label>
              <input
                type="time"
                name="startTime"
                value={form.startTime}
                onChange={handleChange}
                required
                className={inputClass}
              />
            </div>

            <div>
              <label className={labelClass}>
                Special Requests{' '}
                <span className="text-[#C9A8B0] normal-case tracking-normal">(optional)</span>
              </label>
              <textarea
                name="notes"
                value={form.notes}
                onChange={handleChange}
                placeholder="Any special requests or notes for your appointment…"
                rows={3}
                className={inputClass + ' resize-none'}
              />
            </div>

            <div className="pt-2">
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#C96E8A] hover:bg-[#A85471] disabled:bg-[#E5B8C8] text-white font-medium py-3 rounded-full tracking-widest uppercase text-xs transition-colors"
              >
                {loading ? 'Confirming…' : 'Confirm Booking'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
