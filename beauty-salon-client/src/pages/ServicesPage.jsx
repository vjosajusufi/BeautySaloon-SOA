import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';

const FALLBACK_IMAGE = 'https://images.unsplash.com/photo-1560066984-138dadb4c035?w=400';

const SERVICE_IMAGES = {
  haircut: 'https://images.unsplash.com/photo-1522337360788-8b13dee7a37e?w=400',
  color: 'https://images.unsplash.com/photo-1595476108010-b4d1f102b1b1?w=400',
  manicure: 'https://images.unsplash.com/photo-1604654894610-df63bc536371?w=400',
  pedicure: 'https://images.unsplash.com/photo-1519014816548-bf5fe059798b?w=400',
  facial: 'https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?w=400',
  eyebrow: 'https://images.unsplash.com/photo-1616394584738-fc6e612e71b9?w=400',
  fallback: FALLBACK_IMAGE,
};

function getServiceImage(name) {
  const n = name.toLowerCase();
  if (n.includes('haircut') || n.includes('hair cut') || n.includes('trim')) return SERVICE_IMAGES.haircut;
  if (n.includes('color') || n.includes('colour') || n.includes('highlight') || n.includes('dye')) return SERVICE_IMAGES.color;
  if (n.includes('manicure') || n.includes('nail')) return SERVICE_IMAGES.manicure;
  if (n.includes('pedicure') || n.includes('foot')) return SERVICE_IMAGES.pedicure;
  if (n.includes('facial') || n.includes('face') || n.includes('skin')) return SERVICE_IMAGES.facial;
  if (n.includes('eyebrow') || n.includes('threading') || n.includes('brow') || n.includes('wax')) return SERVICE_IMAGES.eyebrow;
  return SERVICE_IMAGES.fallback;
}

export default function ServicesPage() {
  const { user } = useAuth();
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    api
      .get('/services')
      .then(({ data }) => setServices(data))
      .catch(() => setError('Failed to load services.'))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="min-h-screen bg-[#FFF5F7]">

      {/* Hero — full-bleed background image */}
      <div
        className="relative border-b border-[#FFD1DC] py-32 px-4 overflow-hidden"
        style={{
          backgroundImage: 'url(https://images.unsplash.com/photo-1560066984-138dadb4c035?w=1600)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        {/* Dark overlay */}
        <div className="absolute inset-0 bg-[#2A1520]/55" />

        {/* Content */}
        <div className="relative z-10 max-w-3xl mx-auto text-center">
          <p className="text-[#FFD1DC] text-xs tracking-[5px] uppercase mb-5 font-medium">
            Welcome to Bella Beauty Salon
          </p>
          <h1 className="font-display text-5xl md:text-6xl text-white leading-tight mb-5 drop-shadow-lg">
            Timeless Beauty,
            <br />
            <span className="italic">Unparalleled Care</span>
          </h1>
          <div className="flex items-center justify-center gap-3 mb-6">
            <div className="w-12 h-px bg-[#FFB6C1]/70" />
            <span className="text-[#FFB6C1]">✦</span>
            <div className="w-12 h-px bg-[#FFB6C1]/70" />
          </div>
          <p className="text-white/80 text-lg font-light leading-relaxed max-w-xl mx-auto">
            Discover our curated collection of luxury beauty treatments designed
            to rejuvenate your body and elevate your spirit.
          </p>
          {!user && (
            <div className="mt-10 flex items-center justify-center gap-4">
              <Link
                to="/register"
                className="bg-[#C96E8A] hover:bg-[#A85471] text-white px-8 py-3 rounded-full text-sm tracking-widest uppercase transition-colors font-medium shadow-lg"
              >
                Book a Treatment
              </Link>
              <Link
                to="/login"
                className="border border-white/60 text-white hover:bg-white/10 px-8 py-3 rounded-full text-sm tracking-widest uppercase transition-colors font-medium"
              >
                Sign In
              </Link>
            </div>
          )}
        </div>
      </div>

      {/* Services */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="text-center mb-12">
          <p className="text-[#C96E8A] text-xs tracking-[4px] uppercase mb-3 font-medium">
            Our Treatments
          </p>
          <h2 className="font-display text-3xl text-[#3D2430]">
            Signature Services
          </h2>
        </div>

        {loading && (
          <div className="text-center py-20 text-[#C96E8A] text-sm tracking-widest uppercase">
            Loading services…
          </div>
        )}

        {error && (
          <div className="text-center py-20 text-red-400 text-sm">{error}</div>
        )}

        {!loading && !error && services.length === 0 && (
          <div className="text-center py-20 text-[#9B8590] text-sm">
            No services available at the moment.
          </div>
        )}

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-7">
          {services.map((service) => (
            <div
              key={service.id}
              className="bg-white border border-[#FFD1DC] rounded-2xl shadow-[0_4px_30px_rgba(255,182,193,0.15)] hover:shadow-[0_8px_40px_rgba(255,182,193,0.3)] transition-all duration-300 overflow-hidden flex flex-col group"
            >
              {/* Service image */}
              <div className="relative h-48 overflow-hidden">
                <img
                  src={getServiceImage(service.name)}
                  alt={service.name}
                  onError={(e) => { e.currentTarget.onerror = null; e.currentTarget.src = FALLBACK_IMAGE; }}
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                />
                {!service.isActive && (
                  <div className="absolute inset-0 bg-white/70 flex items-center justify-center">
                    <span className="text-xs bg-white text-gray-400 px-3 py-1 rounded-full border border-gray-200 tracking-wider">
                      Unavailable
                    </span>
                  </div>
                )}
              </div>

              <div className="p-7 flex flex-col flex-1">
                <h2 className="font-display text-xl text-[#3D2430] mb-2">
                  {service.name}
                </h2>
                <p className="text-[#6B5560] text-sm font-light leading-relaxed flex-1 mb-5">
                  {service.description}
                </p>
                <div className="flex items-center justify-between border-t border-[#FFD1DC] pt-4 mb-4">
                  <span className="bg-[#FFF5F7] text-[#C96E8A] text-xs px-3 py-1 rounded-full border border-[#FFD1DC] tracking-wider">
                    {service.durationMinutes} min
                  </span>
                  <span className="font-display text-2xl text-[#C96E8A]">
                    ${service.price.toFixed(2)}
                  </span>
                </div>
                {user && service.isActive && (
                  <Link
                    to={`/book?serviceId=${service.id}`}
                    className="block text-center bg-[#C96E8A] hover:bg-[#A85471] text-white text-xs tracking-widest uppercase py-3 rounded-full transition-colors font-medium"
                  >
                    Book Appointment
                  </Link>
                )}
                {!user && service.isActive && (
                  <Link
                    to="/login"
                    className="block text-center border border-[#FFB6C1] text-[#C96E8A] hover:bg-[#FFF5F7] text-xs tracking-widest uppercase py-3 rounded-full transition-colors font-medium"
                  >
                    Sign In to Book
                  </Link>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
