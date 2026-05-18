import { useEffect, useMemo, useState } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';

import Checkout from './components/Checkout';
import Dashboard from './components/Dashboard';
import Login from './components/Login';
import MyTickets from './components/MyTickets';
import type { IFlight } from './types/flight';
import { getFlights } from './services/flightService';

function App() {
  const [flights, setFlights] = useState<IFlight[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadFlights = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await getFlights();
        setFlights(data);
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Bilinmeyen bir hata olustu.';
        setError(message);
      } finally {
        setLoading(false);
      }
    };

    loadFlights();
  }, []);

  const toastStyles = useMemo(
    () => ({
      style: {
        background: '#0f172a',
        color: '#f8fafc',
        border: '1px solid #dc2626',
      },
      iconTheme: {
        primary: '#dc2626',
        secondary: '#0f172a',
      },
    }),
    []
  );

  const renderFlights = () => {
    if (loading) {
      return (
        <div className="flex items-center justify-center min-h-screen bg-slate-50">
          <p className="text-lg font-semibold text-slate-600">Yukleniyor...</p>
        </div>
      );
    }

    return (
      <div className="min-h-screen bg-slate-50 px-6 py-10">
        <h1 className="text-3xl font-bold text-slate-800">Ucus Listesi</h1>

        {error ? (
          <p className="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm font-semibold text-red-600">
            {error}
          </p>
        ) : (
          <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {flights.map((flight) => (
              <div
                key={flight.id}
                className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
              >
                {/* Backward-compatible mapping for older payload shapes */}
                {(() => {
                  const priceValue =
                    typeof flight.price === 'number'
                      ? flight.price
                      : typeof (flight as { basePrice?: number }).basePrice ===
                        'number'
                        ? (flight as { basePrice?: number }).basePrice
                        : null;
                  const priceText =
                    typeof priceValue === 'number'
                      ? priceValue.toFixed(2)
                      : 'Fiyat yok';
                  const departureValue =
                    (flight as { departure?: string }).departure ??
                    (flight as { origin?: string }).origin ??
                    '-';
                  const destinationValue =
                    (flight as { destination?: string }).destination ?? '-';
                  const dateValue =
                    (flight as { date?: string }).date ??
                    (flight as { departureTime?: string }).departureTime ?? '';

                  return (
                    <>
                      <div className="text-sm font-semibold text-slate-500">Ucus</div>
                      <div className="mt-2 text-lg font-semibold text-slate-800">
                        {departureValue} → {destinationValue}
                      </div>
                      <div className="mt-2 text-sm text-slate-500">
                        Kalkis:{' '}
                        {dateValue
                          ? new Date(dateValue).toLocaleString('tr-TR')
                          : 'Tarih bilgisi yok'}
                      </div>
                      <div className="mt-4 text-base font-bold text-indigo-600">
                        {priceText} TRY
                      </div>
                    </>
                  );
                })()}
              </div>
            ))}
          </div>
        )}
      </div>
    );
  };

  return (
    <Router>
      <Toaster position="top-right" toastOptions={toastStyles} />

      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/my-tickets" element={<MyTickets />} />
        <Route path="/checkout/:id" element={<Checkout />} />
        <Route path="/" element={renderFlights()} />
      </Routes>
    </Router>
  );
}

export default App;