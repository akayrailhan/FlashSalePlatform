import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    ArrowRight,
    Loader2,
    LogOut,
    Plane,
    PlaneTakeoff,
    Ticket,
} from 'lucide-react';
import { supabaseClient } from '../services/supabaseClient';
import { getFlights } from '../services/flightService';
import type { IFlight } from '../types/flight';

function Dashboard() {
    const navigate = useNavigate();
    const [flights, setFlights] = useState<IFlight[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadFlights = async () => {
            try {
                setLoading(true);
                setError(null);
                // Data fetching stays in services to keep UI lean.
                const data = await getFlights();
                setFlights(data);
            } catch (err) {
                const message =
                    err instanceof Error ? err.message : 'Ucuslar yuklenemedi.';
                setError(message);
            } finally {
                setLoading(false);
            }
        };

        loadFlights();
    }, []);

    const handleLogout = async () => {
        await supabaseClient.auth.signOut();
        navigate('/login');
    };

    return (
        <div className="min-h-screen bg-slate-50 px-6 py-10 text-slate-900">
            <div className="mx-auto max-w-5xl rounded-3xl border border-slate-200 bg-white p-8 shadow-xl">
                <header className="flex flex-col gap-4 border-b border-slate-100 pb-6 md:flex-row md:items-center md:justify-between">
                    <div className="flex items-center gap-3">
                        <Plane className="h-8 w-8 text-red-600" />
                        <div>
                            <p className="text-lg font-semibold text-slate-900">GalaJet</p>
                            <p className="text-xs uppercase tracking-[0.3em] text-red-500">
                                Premium Flight Booking
                            </p>
                        </div>
                    </div>
                    <div className="flex flex-col gap-3 sm:flex-row">
                        <button
                            type="button"
                            onClick={() => navigate('/my-tickets')}
                            className="flex items-center gap-2 rounded-full border border-zinc-200 bg-zinc-50 px-4 py-2 text-xs font-semibold text-zinc-700 transition hover:border-red-200 hover:bg-red-50 hover:text-red-700"
                        >
                            <ArrowRight className="h-4 w-4" />
                            Biletlerim
                        </button>
                        <button
                            type="button"
                            onClick={handleLogout}
                            className="flex items-center gap-2 rounded-full border border-red-100 bg-red-50 px-4 py-2 text-xs font-semibold text-red-600 transition hover:border-red-200 hover:bg-red-100"
                        >
                            <LogOut className="h-4 w-4" />
                            Cikis Yap
                        </button>
                    </div>
                </header>

                <section className="mt-8">
                    <h1 className="text-3xl font-bold text-slate-900">
                        Hos Geldin, Kaptan! 🧑‍✈️
                    </h1>
                    <p className="mt-2 text-sm text-slate-500">
                        Ucus planlarini ve biletlerini tek bir yerden yonet.
                    </p>

                    {loading ? (
                        <div className="mt-8 flex min-h-[260px] items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-slate-50 p-8">
                            <div className="flex items-center gap-3 text-sm font-semibold text-slate-600">
                                <Loader2 className="h-5 w-5 animate-spin text-red-600" />
                                Ucuslar yukleniyor...
                            </div>
                        </div>
                    ) : null}

                    {error ? (
                        <div className="mt-6 rounded-2xl border border-red-100 bg-red-50 px-4 py-3 text-sm font-semibold text-red-600">
                            {error}
                        </div>
                    ) : null}

                    {!loading && !error ? (
                        <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
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
                                            (flight as { destination?: string }).destination ??
                                            '-';
                                        const dateValue =
                                            (flight as { date?: string }).date ??
                                            (flight as { departureTime?: string }).departureTime ??
                                            '';

                                        return (
                                            <>
                                                <div className="flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-red-500">
                                                    <PlaneTakeoff className="h-4 w-4" />
                                                    Ucus
                                                </div>
                                                <div className="mt-3 text-lg font-semibold text-slate-900">
                                                    {departureValue} → {destinationValue}
                                                </div>
                                                <div className="mt-2 text-sm text-slate-500">
                                                    Tarih:{' '}
                                                    {dateValue
                                                        ? new Date(dateValue).toLocaleString('tr-TR')
                                                        : 'Tarih bilgisi yok'}
                                                </div>
                                                <div className="mt-4 text-xl font-bold text-red-600">
                                                    {priceText} TRY
                                                </div>
                                            </>
                                        );
                                    })()}
                                    <button
                                        type="button"
                                        onClick={() => navigate(`/checkout/${flight.id}`)}
                                        className="mt-4 flex w-full items-center justify-center gap-2 rounded-xl bg-red-600 px-4 py-2 text-xs font-semibold text-white transition hover:bg-red-700"
                                    >
                                        <Ticket className="h-4 w-4 text-red-400" />
                                        Bilet Al
                                    </button>
                                </div>
                            ))}
                        </div>
                    ) : null}
                </section>
            </div>
        </div>
    );
}

export default Dashboard;
