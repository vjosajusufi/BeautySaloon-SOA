import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';

const STATUS_STYLES = {
  Pending: 'bg-[#FFD1DC] text-[#A85471]',
  Confirmed: 'bg-green-100 text-green-700',
  Cancelled: 'bg-gray-100 text-gray-500',
  Completed: 'bg-blue-50 text-blue-600',
};

export default function MyAppointmentsPage() {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    api
      .get('/appointments/my')
      .then(({ data }) => {
        const sorted = [...data].sort(
          (a, b) => new Date(b.appointmentDate) - new Date(a.appointmentDate)
        );
        setAppointments(sorted);
      })
      .catch(() => setError('Failed to load appointments.'))
      .finally(() => setLoading(false));
  }, []);

  async function handleCancel(id) {
    if (!confirm('Cancel this appointment?')) return;
    try {
      await api.delete(`/appointments/${id}`);
      setAppointments((prev) => prev.filter((a) => a.id !== id));
    } catch {
      alert('Failed to cancel appointment.');
    }
  }

  return (
    <div className="min-h-screen bg-[#FFF5F7] py-16 px-4">
      <div className="max-w-3xl mx-auto">

        {/* Header */}
        <div className="flex items-end justify-between mb-10">
          <div>
            <p className="text-[#C96E8A] text-xs tracking-[4px] uppercase mb-2 font-medium">
              Your Schedule
            </p>
            <h1 className="font-display text-4xl text-[#3D2430]">
              My Appointments
            </h1>
          </div>
          <Link
            to="/book"
            className="bg-[#C96E8A] hover:bg-[#A85471] text-white px-5 py-2.5 rounded-full text-xs tracking-widest uppercase transition-colors font-medium"
          >
            + Book New
          </Link>
        </div>

        {loading && (
          <div className="text-center py-20 text-[#C96E8A] text-xs tracking-widest uppercase">
            Loading appointments…
          </div>
        )}

        {error && (
          <div className="text-center py-20 text-red-400 text-sm">{error}</div>
        )}

        {!loading && !error && appointments.length === 0 && (
          <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_30px_rgba(255,182,193,0.15)] p-14 text-center">
            <p className="text-[#FFB6C1] text-4xl mb-5">✦</p>
            <h2 className="font-display text-2xl text-[#3D2430] mb-2 italic">
              No appointments yet
            </h2>
            <p className="text-[#9B8590] text-sm font-light mb-8">
              Treat yourself to one of our luxury services
            </p>
            <Link
              to="/book"
              className="inline-block bg-[#C96E8A] hover:bg-[#A85471] text-white px-8 py-3 rounded-full text-xs tracking-widest uppercase transition-colors font-medium"
            >
              Book Now
            </Link>
          </div>
        )}

        <div className="space-y-4">
          {appointments.map((appt) => (
            <div
              key={appt.id}
              className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_20px_rgba(255,182,193,0.12)] p-6 hover:shadow-[0_6px_30px_rgba(255,182,193,0.2)] transition-shadow"
            >
              <div className="flex items-start justify-between gap-4">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-3 mb-3 flex-wrap">
                    <h3 className="font-display text-xl text-[#3D2430]">
                      {appt.serviceName}
                    </h3>
                    <span
                      className={`text-xs font-medium px-3 py-0.5 rounded-full tracking-wider ${STATUS_STYLES[appt.status] ?? 'bg-gray-100 text-gray-500'}`}
                    >
                      {appt.status}
                    </span>
                  </div>
                  <div className="flex flex-wrap gap-5 text-sm text-[#6B5560]">
                    <span className="flex items-center gap-1.5">
                      <span className="text-[#FFB6C1]">◆</span>
                      {appt.appointmentDate}
                    </span>
                    <span className="flex items-center gap-1.5">
                      <span className="text-[#FFB6C1]">◆</span>
                      {appt.startTime.slice(0, 5)} – {appt.endTime.slice(0, 5)}
                    </span>
                  </div>
                  {appt.notes && (
                    <p className="text-sm text-[#9B8590] mt-3 italic font-light">
                      &ldquo;{appt.notes}&rdquo;
                    </p>
                  )}
                </div>

                {appt.status === 'Pending' && (
                  <button
                    onClick={() => handleCancel(appt.id)}
                    className="text-xs text-[#C96E8A] hover:text-[#A85471] border border-[#FFD1DC] hover:border-[#FFB6C1] px-4 py-2 rounded-full transition-colors whitespace-nowrap tracking-wider shrink-0"
                  >
                    Cancel
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
