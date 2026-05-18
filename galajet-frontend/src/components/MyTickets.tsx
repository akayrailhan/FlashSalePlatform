import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AlertTriangle, Loader2, Plane, Ticket, ArrowLeft, CalendarClock } from 'lucide-react';
import apiClient from '../services/apiClient';
import { supabaseClient } from '../services/supabaseClient';

type BookingDto = {
    pnrCode: string;
    flightId: string;
    createdAt: string;
};

function MyTickets() {
    const navigate = useNavigate();
    const [tickets, setTickets] = useState<BookingDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadTickets = async () => {
            try {
                setIsLoading(true);
                setError(null);

                const { data: sessionData } = await supabaseClient.auth.getSession();
                const token = sessionData.session?.access_token;

                if (!token) {
                    throw new Error('Oturum bulunamadi. Lutfen tekrar giris yapin.');
                }

                const response = await apiClient.get<BookingDto[]>('/api/bookings', {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                setTickets(response.data ?? []);
            } catch (err) {
                const message =
                    err instanceof Error ? err.message : 'Biletler yuklenirken bir hata olustu.';
                setError(message);
            } finally {
                setIsLoading(false);
            }
        };

        loadTickets();
    }, []);

    return (
        <div className="min-h-screen bg-white px-6 py-10 text-slate-900">
            <div className="mx-auto max-w-5xl">
                <header className="mb-8 flex flex-col gap-4 rounded-3xl border border-slate-200 bg-white p-6 shadow-sm md:flex-row md:items-center md:justify-between">
                    <div className="flex items-center gap-3">
                        <Plane className="h-8 w-8 text-red-500" />
                        <div>
                            <p className="text-lg font-semibold text-slate-900">GalaJet</p>
                            <p className="text-xs uppercase tracking-[0.28em] text-red-500">
                                Biletlerim
                            </p>
                        </div>
                    </div>
                    <button
                        type="button"
                        onClick={() => navigate('/dashboard')}
                        className="inline-flex items-center gap-2 rounded-full border border-red-100 bg-red-50 px-4 py-2 text-xs font-semibold text-red-600 transition hover:border-red-200 hover:bg-red-100"
                    >
                        <ArrowLeft className="h-4 w-4" />
                        Panele Don
                    </button>
                </header>

                {isLoading ? (
                    <div className="flex min-h-[280px] items-center justify-center rounded-3xl border border-slate-200 bg-slate-50">
                        <div className="flex items-center gap-3 text-sm font-semibold text-slate-600">
                            <Loader2 className="h-5 w-5 animate-spin text-red-500" />
                            Biletlerin yukleniyor...
                        </div>
                    </div>
                ) : null}

                {!isLoading && error ? (
                    <div className="rounded-2xl border border-red-100 bg-red-50 px-4 py-3 text-sm font-semibold text-red-700">
                        <div className="flex items-center gap-2">
                            <AlertTriangle className="h-4 w-4" />
                            {error}
                        </div>
                    </div>
                ) : null}

                {!isLoading && !error && tickets.length === 0 ? (
                    <div className="rounded-3xl border border-dashed border-slate-300 bg-slate-50 px-6 py-12 text-center">
                        <Ticket className="mx-auto h-10 w-10 text-red-500" />
                        <h2 className="mt-4 text-xl font-bold text-slate-900">Henüz bir uçuşunuz bulunmuyor.</h2>
                        <p className="mt-2 text-sm text-slate-500">
                            Hemen bir rota secin ve GalaJet deneyimiyle yerinizi ayirtin.
                        </p>
                        <button
                            type="button"
                            onClick={() => navigate('/dashboard')}
                            className="mt-6 rounded-xl bg-red-600 px-5 py-2 text-sm font-semibold text-white transition hover:bg-red-700"
                        >
                            Ucuslari Kesfet
                        </button>
                    </div>
                ) : null}

                {!isLoading && !error && tickets.length > 0 ? (
                    <section className="grid gap-5 sm:grid-cols-2">
                        {tickets.map((ticket) => (
                            <article
                                key={`${ticket.pnrCode}-${ticket.flightId}`}
                                className="relative overflow-hidden rounded-3xl border border-red-900/40 bg-zinc-900 shadow-[0_16px_30px_rgba(0,0,0,0.45)]"
                            >
                                <div className="absolute inset-y-0 left-[72%] w-px border-l border-dashed border-red-700/60" />
                                <div className="absolute -left-3 top-[calc(50%-12px)] h-6 w-6 rounded-full border border-red-800/60 bg-black" />
                                <div className="absolute -right-3 top-[calc(50%-12px)] h-6 w-6 rounded-full border border-red-800/60 bg-black" />

                                <div className="grid grid-cols-1 gap-0 sm:grid-cols-[3fr_1.2fr]">
                                    <div className="p-6">
                                        <p className="text-[11px] font-semibold uppercase tracking-[0.24em] text-red-400">
                                            GalaJet Binis Karti
                                        </p>
                                        <h3 className="mt-3 text-3xl font-black tracking-[0.08em] text-white">
                                            {ticket.pnrCode}
                                        </h3>
                                        <div className="mt-5 flex items-center gap-2 text-sm text-zinc-300">
                                            <Ticket className="h-4 w-4 text-red-500" />
                                            Ucus Id: <span className="font-semibold text-zinc-200">{ticket.flightId}</span>
                                        </div>
                                        <div className="mt-2 flex items-center gap-2 text-sm text-zinc-400">
                                            <CalendarClock className="h-4 w-4 text-red-500" />
                                            Satin Alma: {new Date(ticket.createdAt).toLocaleString('tr-TR')}
                                        </div>
                                    </div>

                                    <div className="flex flex-col justify-center bg-red-950/40 p-6 text-center">
                                        <p className="text-[10px] font-bold uppercase tracking-[0.3em] text-red-300">
                                            PNR
                                        </p>
                                        <p className="mt-2 text-2xl font-black text-white">{ticket.pnrCode}</p>
                                    </div>
                                </div>
                            </article>
                        ))}
                    </section>
                ) : null}
            </div>
        </div>
    );
}

export default MyTickets;
