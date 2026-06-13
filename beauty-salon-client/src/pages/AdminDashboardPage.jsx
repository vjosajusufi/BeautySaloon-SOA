import { useEffect, useState } from 'react';
import api from '../api/axios';

const STATUS_STYLES = {
  Pending: 'bg-[#FFD1DC] text-[#A85471]',
  Confirmed: 'bg-green-100 text-green-700',
  Cancelled: 'bg-gray-100 text-gray-500',
  Completed: 'bg-blue-50 text-blue-600',
};

const EMPTY_SERVICE = { name: '', description: '', price: '', durationMinutes: '' };

const inputClass =
  'w-full border border-[#FFD1DC] rounded-xl px-3 py-2.5 text-sm text-[#3D2430] placeholder-[#C9A8B0] focus:outline-none focus:border-[#FFB6C1] focus:ring-2 focus:ring-[#FFB6C1]/20 transition-all bg-white';
const labelClass =
  'block text-xs tracking-[2px] uppercase text-[#6B5560] mb-1.5 font-medium';

export default function AdminDashboardPage() {
  const [tab, setTab] = useState('appointments');

  const [appointments, setAppointments] = useState([]);
  const [apptLoading, setApptLoading] = useState(true);
  const [apptError, setApptError] = useState('');

  const [services, setServices] = useState([]);
  const [svcLoading, setSvcLoading] = useState(true);
  const [svcError, setSvcError] = useState('');

  const [serviceForm, setServiceForm] = useState(EMPTY_SERVICE);
  const [editingId, setEditingId] = useState(null);
  const [formError, setFormError] = useState('');
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => { loadAppointments(); }, []);
  useEffect(() => { loadServices(); }, []);

  function loadAppointments() {
    setApptLoading(true);
    setApptError('');
    api
      .get('/appointments')
      .then(({ data }) => {
        const sorted = [...data].sort(
          (a, b) => new Date(b.appointmentDate) - new Date(a.appointmentDate)
        );
        setAppointments(sorted);
      })
      .catch(() => setApptError('Failed to load appointments.'))
      .finally(() => setApptLoading(false));
  }

  function loadServices() {
    setSvcLoading(true);
    api
      .get('/services')
      .then(({ data }) => setServices(data))
      .catch(() => setSvcError('Failed to load services.'))
      .finally(() => setSvcLoading(false));
  }

  function handleSvcFormChange(e) {
    setServiceForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  function startEdit(service) {
    setEditingId(service.id);
    setServiceForm({
      name: service.name,
      description: service.description,
      price: service.price.toString(),
      durationMinutes: service.durationMinutes.toString(),
    });
    setFormError('');
  }

  function cancelEdit() {
    setEditingId(null);
    setServiceForm(EMPTY_SERVICE);
    setFormError('');
  }

  async function handleServiceSubmit(e) {
    e.preventDefault();
    setFormError('');
    setFormLoading(true);
    const payload = {
      name: serviceForm.name,
      description: serviceForm.description,
      price: parseFloat(serviceForm.price),
      durationMinutes: parseInt(serviceForm.durationMinutes, 10),
    };
    try {
      if (editingId) {
        const { data } = await api.put(`/services/${editingId}`, payload);
        setServices((prev) => prev.map((s) => (s.id === editingId ? data : s)));
      } else {
        const { data } = await api.post('/services', payload);
        setServices((prev) => [...prev, data]);
      }
      cancelEdit();
    } catch (err) {
      setFormError(err.response?.data?.message || 'Failed to save service.');
    } finally {
      setFormLoading(false);
    }
  }

  async function handleCancelAppointment(id) {
    if (!confirm('Cancel this appointment?')) return;
    try {
      await api.delete(`/appointments/${id}`);
      loadAppointments();
    } catch {
      alert('Failed to cancel appointment.');
    }
  }

  async function handleConfirmAppointment(appt) {
    console.log('Confirming appointment:', appt);
    console.log('Appointment id:', appt.id);
    try {
      console.log('Sending PUT to:', `/appointments/${appt.id}`);
      const response = await api.put(`/appointments/${appt.id}`, {
        userId: appt.userId,
        serviceId: appt.serviceId,
        appointmentDate: appt.appointmentDate,
        startTime: appt.startTime,
        notes: appt.notes ?? null,
        status: 'Confirmed',
      });
      console.log('Response:', response);
      setAppointments(prev => prev.map(a => a.id === appt.id ? { ...a, status: 'Confirmed' } : a));
    } catch (error) {
      console.error('Full error:', error);
      console.error('Error response:', error.response?.data);
      alert('Failed to confirm appointment.');
    }
  }

  async function handleDeleteService(id) {
    if (!confirm('Delete this service?')) return;
    try {
      await api.delete(`/services/${id}`);
      setServices((prev) => prev.filter((s) => s.id !== id));
    } catch {
      alert('Failed to delete service.');
    }
  }

  return (
    <div className="min-h-screen bg-[#FFF5F7] py-12 px-4">
      <div className="max-w-6xl mx-auto">

        {/* Page Header */}
        <div className="mb-10">
          <p className="text-[#C96E8A] text-xs tracking-[4px] uppercase mb-2 font-medium">
            Management
          </p>
          <h1 className="font-display text-4xl text-[#3D2430]">
            Admin Dashboard
          </h1>
        </div>

        {/* Tabs */}
        <div className="flex gap-3 mb-8">
          <button
            onClick={() => setTab('appointments')}
            className={`px-6 py-2.5 rounded-full text-xs tracking-[2px] uppercase font-medium transition-all ${
              tab === 'appointments'
                ? 'bg-[#C96E8A] text-white shadow-[0_4px_15px_rgba(201,110,138,0.3)]'
                : 'bg-white border border-[#FFD1DC] text-[#6B5560] hover:border-[#FFB6C1] hover:text-[#C96E8A]'
            }`}
          >
            All Appointments
          </button>
          <button
            onClick={() => setTab('services')}
            className={`px-6 py-2.5 rounded-full text-xs tracking-[2px] uppercase font-medium transition-all ${
              tab === 'services'
                ? 'bg-[#C96E8A] text-white shadow-[0_4px_15px_rgba(201,110,138,0.3)]'
                : 'bg-white border border-[#FFD1DC] text-[#6B5560] hover:border-[#FFB6C1] hover:text-[#C96E8A]'
            }`}
          >
            Manage Services
          </button>
        </div>

        {/* Appointments Tab */}
        {tab === 'appointments' && (
          <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_30px_rgba(255,182,193,0.15)] overflow-hidden">
            <div className="px-6 py-5 border-b border-[#FFD1DC] flex items-center justify-between">
              <h2 className="font-display text-xl text-[#3D2430]">
                All Appointments
              </h2>
              <span className="text-[#C96E8A] text-xs tracking-wider bg-[#FFF5F7] border border-[#FFD1DC] px-3 py-1 rounded-full">
                {appointments.length} total
              </span>
            </div>

            {apptLoading && (
              <div className="text-center py-14 text-[#C96E8A] text-xs tracking-widest uppercase">
                Loading…
              </div>
            )}
            {apptError && (
              <div className="text-center py-14 text-red-400 text-sm">{apptError}</div>
            )}
            {!apptLoading && !apptError && appointments.length === 0 && (
              <div className="text-center py-14 text-[#9B8590] text-sm">
                No appointments found.
              </div>
            )}

            {appointments.length > 0 && (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="bg-[#FFF5F7] border-b border-[#FFD1DC]">
                      {['Client', 'Service', 'Date', 'Time', 'Status', 'Notes', 'Actions'].map((h) => (
                        <th key={h} className="px-5 py-3.5 text-left text-xs tracking-[2px] uppercase text-[#9B8590] font-medium">
                          {h}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {appointments.map((appt, i) => (
                      <tr
                        key={appt.id}
                        className={`border-b border-[#FFD1DC]/40 hover:bg-[#FFF5F7] transition-colors ${i % 2 === 1 ? 'bg-[#FFFBFC]' : 'bg-white'}`}
                      >
                        <td className="px-5 py-4 font-medium text-[#3D2430]">
                          {appt.userFullName}
                        </td>
                        <td className="px-5 py-4 text-[#6B5560]">{appt.serviceName}</td>
                        <td className="px-5 py-4 text-[#6B5560]">{appt.appointmentDate}</td>
                        <td className="px-5 py-4 text-[#6B5560]">
                          {appt.startTime.slice(0, 5)} – {appt.endTime.slice(0, 5)}
                        </td>
                        <td className="px-5 py-4">
                          <span className={`text-xs font-medium px-3 py-0.5 rounded-full tracking-wider ${STATUS_STYLES[appt.status] ?? 'bg-gray-100 text-gray-500'}`}>
                            {appt.status}
                          </span>
                        </td>
                        <td className="px-5 py-4 text-[#9B8590] italic text-xs max-w-[160px] truncate">
                          {appt.notes || '—'}
                        </td>
                        <td className="px-5 py-4">
                          <div className="flex gap-2">
                            {appt.status === 'Pending' && (
                              <button
                                onClick={() => handleConfirmAppointment(appt)}
                                className="text-xs text-green-600 hover:text-green-800 border border-green-200 hover:border-green-300 px-3 py-1 rounded-full transition-colors whitespace-nowrap tracking-wider"
                              >
                                Confirm
                              </button>
                            )}
                            {(appt.status === 'Pending' || appt.status === 'Confirmed') && (
                              <button
                                onClick={() => handleCancelAppointment(appt.id)}
                                className="text-xs text-red-400 hover:text-red-600 border border-red-200 hover:border-red-300 px-3 py-1 rounded-full transition-colors whitespace-nowrap tracking-wider"
                              >
                                Cancel
                              </button>
                            )}
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}

        {/* Services Tab */}
        {tab === 'services' && (
          <div className="grid grid-cols-1 lg:grid-cols-5 gap-6">

            {/* Services List */}
            <div className="lg:col-span-3 bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_30px_rgba(255,182,193,0.15)] overflow-hidden">
              <div className="px-6 py-5 border-b border-[#FFD1DC] flex items-center justify-between">
                <h2 className="font-display text-xl text-[#3D2430]">Services</h2>
                <span className="text-[#C96E8A] text-xs tracking-wider bg-[#FFF5F7] border border-[#FFD1DC] px-3 py-1 rounded-full">
                  {services.length} total
                </span>
              </div>

              {svcLoading && (
                <div className="text-center py-12 text-[#C96E8A] text-xs tracking-widest uppercase">Loading…</div>
              )}
              {svcError && (
                <div className="text-center py-12 text-red-400 text-sm">{svcError}</div>
              )}

              <div className="divide-y divide-[#FFD1DC]/40">
                {services.map((service) => (
                  <div
                    key={service.id}
                    className={`px-6 py-4 flex items-center justify-between gap-4 hover:bg-[#FFF5F7] transition-colors ${editingId === service.id ? 'bg-[#FFF5F7]' : ''}`}
                  >
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-0.5">
                        <span className="font-medium text-[#3D2430] text-sm">
                          {service.name}
                        </span>
                        {!service.isActive && (
                          <span className="text-xs bg-gray-100 text-gray-400 px-2 py-0.5 rounded-full">
                            Inactive
                          </span>
                        )}
                      </div>
                      <p className="text-xs text-[#9B8590] truncate font-light">
                        {service.description}
                      </p>
                      <div className="flex gap-3 text-xs text-[#6B5560] mt-1">
                        <span>{service.durationMinutes} min</span>
                        <span className="text-[#C96E8A] font-medium">
                          ${service.price.toFixed(2)}
                        </span>
                      </div>
                    </div>
                    <div className="flex gap-2 shrink-0">
                      <button
                        onClick={() => startEdit(service)}
                        className="text-xs text-[#C96E8A] hover:text-[#A85471] border border-[#FFD1DC] hover:border-[#FFB6C1] px-3 py-1.5 rounded-full transition-colors tracking-wider"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteService(service.id)}
                        className="text-xs text-red-400 hover:text-red-600 border border-red-200 hover:border-red-300 px-3 py-1.5 rounded-full transition-colors tracking-wider"
                      >
                        Delete
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Service Form */}
            <div className="lg:col-span-2">
              <div className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_30px_rgba(255,182,193,0.15)] p-6 sticky top-6">
                <h2 className="font-display text-xl text-[#3D2430] mb-1">
                  {editingId ? 'Edit Service' : 'Add New Service'}
                </h2>
                <div className="w-8 h-px bg-[#FFD1DC] mb-5" />

                {formError && (
                  <div className="bg-red-50 border border-red-200 text-red-500 rounded-xl px-3 py-2.5 text-xs mb-4">
                    {formError}
                  </div>
                )}

                <form onSubmit={handleServiceSubmit} className="space-y-4">
                  <div>
                    <label className={labelClass}>Name</label>
                    <input
                      type="text"
                      name="name"
                      value={serviceForm.name}
                      onChange={handleSvcFormChange}
                      required
                      placeholder="e.g. Classic Manicure"
                      className={inputClass}
                    />
                  </div>

                  <div>
                    <label className={labelClass}>Description</label>
                    <textarea
                      name="description"
                      value={serviceForm.description}
                      onChange={handleSvcFormChange}
                      required
                      rows={3}
                      placeholder="Brief description of the service…"
                      className={inputClass + ' resize-none'}
                    />
                  </div>

                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className={labelClass}>Price ($)</label>
                      <input
                        type="number"
                        name="price"
                        value={serviceForm.price}
                        onChange={handleSvcFormChange}
                        required
                        min="0"
                        step="0.01"
                        placeholder="0.00"
                        className={inputClass}
                      />
                    </div>
                    <div>
                      <label className={labelClass}>Duration (min)</label>
                      <input
                        type="number"
                        name="durationMinutes"
                        value={serviceForm.durationMinutes}
                        onChange={handleSvcFormChange}
                        required
                        min="1"
                        placeholder="60"
                        className={inputClass}
                      />
                    </div>
                  </div>

                  <div className="flex gap-2 pt-2">
                    {editingId && (
                      <button
                        type="button"
                        onClick={cancelEdit}
                        className="flex-1 border border-[#FFD1DC] text-[#6B5560] hover:bg-[#FFF5F7] py-2.5 rounded-full text-xs tracking-widest uppercase font-medium transition-colors"
                      >
                        Cancel
                      </button>
                    )}
                    <button
                      type="submit"
                      disabled={formLoading}
                      className="flex-1 bg-[#C96E8A] hover:bg-[#A85471] disabled:bg-[#E5B8C8] text-white py-2.5 rounded-full text-xs tracking-widest uppercase font-medium transition-colors"
                    >
                      {formLoading ? 'Saving…' : editingId ? 'Save Changes' : 'Add Service'}
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
