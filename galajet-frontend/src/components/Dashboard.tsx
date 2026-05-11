import { useNavigate } from 'react-router-dom';
import { LogOut, Plane, Ticket } from 'lucide-react';
import { supabaseClient } from '../services/supabaseClient';

function Dashboard() {
    const navigate = useNavigate();

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
                    <button
                        type="button"
                        onClick={handleLogout}
                        className="flex items-center gap-2 rounded-full border border-red-100 bg-red-50 px-4 py-2 text-xs font-semibold text-red-600 transition hover:border-red-200 hover:bg-red-100"
                    >
                        <LogOut className="h-4 w-4" />
                        Cikis Yap
                    </button>
                </header>

                <section className="mt-8">
                    <h1 className="text-3xl font-bold text-slate-900">
                        Hos Geldin, Kaptan! 🧑‍✈️
                    </h1>
                    <p className="mt-2 text-sm text-slate-500">
                        Ucus planlarini ve biletlerini tek bir yerden yonet.
                    </p>

                    <div className="mt-8 flex min-h-[260px] items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-slate-50 p-8">
                        <div className="text-center">
                            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-red-50">
                                <Ticket className="h-6 w-6 text-red-600" />
                            </div>
                            <p className="mt-4 text-sm font-semibold text-slate-700">
                                Ucus listesi ve arama motoru buraya yüklenecek...
                            </p>
                            <p className="mt-2 text-xs text-slate-500">
                                Azure .NET API entegrasyonundan sonra burasi canlanacak.
                            </p>
                        </div>
                    </div>
                </section>
            </div>
        </div>
    );
}

export default Dashboard;
