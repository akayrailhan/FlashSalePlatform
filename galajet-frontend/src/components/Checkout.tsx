import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
    Calendar,
    CheckCircle,
    CreditCard,
    Loader2,
    Plane,
} from 'lucide-react';
import apiClient from '../services/apiClient';
import { supabaseClient } from '../services/supabaseClient';

const generatePnr = () => {
    const randomPart = Math.random().toString(36).slice(2, 6).toUpperCase();
    return `GLJ-${randomPart}`;
};

function Checkout() {
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const [cardholderName, setCardholderName] = useState('');
    const [cardNumber, setCardNumber] = useState('');
    const [expiryDate, setExpiryDate] = useState('');
    const [cvv, setCvv] = useState('');
    const [isProcessing, setIsProcessing] = useState(false);
    const [successPnr, setSuccessPnr] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const handlePayment = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!id) {
            setErrorMessage('Ucus bilgisi bulunamadi. Lutfen tekrar deneyin.');
            return;
        }

        setErrorMessage(null);
        setIsProcessing(true);

        try {
            // Simulated bank approval delay to keep UX realistic.
            await new Promise((resolve) => setTimeout(resolve, 2000));

            const { data, error } = await supabaseClient.auth.getSession();
            if (error) {
                throw error;
            }

            const token = data.session?.access_token;
            if (!token) {
                throw new Error('Oturum bulunamadi. Lutfen tekrar giris yapin.');
            }

            // API calls stay in the service layer; here we only send intent.
            await apiClient.post(
                '/api/bookings',
                { flightId: id },
                { headers: { Authorization: `Bearer ${token}` } }
            );

            setSuccessPnr(generatePnr());
        } catch (err) {
            const message = err instanceof Error ? err.message : 'Odeme basarisiz oldu.';
            setErrorMessage(message);
        } finally {
            setIsProcessing(false);
        }
    };

    if (successPnr) {
        return (
            <div className="min-h-screen bg-slate-50 px-6 py-12 text-slate-900">
                <div className="mx-auto max-w-3xl rounded-3xl border border-slate-200 bg-white p-10 text-center shadow-xl">
                    <CheckCircle className="mx-auto h-14 w-14 text-emerald-500" />
                    <h1 className="mt-4 text-2xl font-bold text-slate-900">
                        Biletiniz basariyla olusturuldu!
                    </h1>
                    <p className="mt-2 text-sm text-slate-600">
                        PNR Kodunuz: <span className="font-semibold text-red-600">{successPnr}</span>
                    </p>
                    <button
                        type="button"
                        onClick={() => navigate('/dashboard')}
                        className="mt-8 inline-flex items-center justify-center rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white transition hover:bg-slate-950"
                    >
                        Dashboard'a Don
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50 px-6 py-12 text-slate-900">
            <div className="mx-auto grid max-w-6xl gap-6 md:grid-cols-3">
                <div className="rounded-3xl border border-slate-200 bg-white p-8 shadow-xl md:col-span-2">
                    <div className="flex items-center gap-3">
                        <CreditCard className="h-6 w-6 text-red-600" />
                        <h1 className="text-xl font-bold text-slate-900">Odeme Bilgileri</h1>
                    </div>
                    <p className="mt-2 text-sm text-slate-500">
                        Guvenli odeme icin kart bilgilerinizi girin.
                    </p>

                    {errorMessage ? (
                        <div className="mt-6 rounded-2xl border border-red-100 bg-red-50 px-4 py-3 text-sm font-semibold text-red-600">
                            {errorMessage}
                        </div>
                    ) : null}

                    <form onSubmit={handlePayment} className="mt-8 grid gap-6">
                        <div className="grid gap-2">
                            <label className="text-sm font-semibold text-slate-900">
                                Kart Sahibi Adi Soyadi
                            </label>
                            <input
                                value={cardholderName}
                                onChange={(event) => setCardholderName(event.target.value)}
                                placeholder="Ornek: Elif Yilmaz"
                                className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-red-500"
                                required
                            />
                        </div>

                        <div className="grid gap-2">
                            <label className="text-sm font-semibold text-slate-900">Kart Numarasi</label>
                            <input
                                value={cardNumber}
                                onChange={(event) => setCardNumber(event.target.value)}
                                placeholder="1234 5678 9012 3456"
                                maxLength={19}
                                className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-red-500"
                                required
                            />
                        </div>

                        <div className="grid gap-6 sm:grid-cols-2">
                            <div className="grid gap-2">
                                <label className="text-sm font-semibold text-slate-900">Son Kullanma</label>
                                <input
                                    value={expiryDate}
                                    onChange={(event) => setExpiryDate(event.target.value)}
                                    placeholder="MM/YY"
                                    maxLength={5}
                                    className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-red-500"
                                    required
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-semibold text-slate-900">CVV</label>
                                <input
                                    value={cvv}
                                    onChange={(event) => setCvv(event.target.value)}
                                    placeholder="123"
                                    maxLength={4}
                                    className="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900 outline-none focus:border-red-500"
                                    required
                                />
                            </div>
                        </div>

                        <button
                            type="submit"
                            disabled={isProcessing}
                            className="flex items-center justify-center gap-2 rounded-xl bg-red-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-70"
                        >
                            {isProcessing ? <Loader2 className="h-5 w-5 animate-spin" /> : null}
                            Odemeyi Tamamla
                        </button>
                    </form>
                </div>

                <div className="rounded-3xl border border-slate-200 bg-white p-8 shadow-xl">
                    <div className="flex items-center gap-3">
                        <Plane className="h-6 w-6 text-red-600" />
                        <h2 className="text-lg font-semibold text-slate-900">Ucus Ozeti</h2>
                    </div>

                    <div className="mt-6 space-y-4 text-sm text-slate-600">
                        <div className="flex items-center gap-3">
                            <Plane className="h-4 w-4 text-red-500" />
                            <div>
                                <p className="text-xs uppercase text-slate-400">Rota</p>
                                <p className="font-semibold text-slate-900">Istanbul → Amsterdam</p>
                            </div>
                        </div>
                        <div className="flex items-center gap-3">
                            <Calendar className="h-4 w-4 text-red-500" />
                            <div>
                                <p className="text-xs uppercase text-slate-400">Tarih</p>
                                <p className="font-semibold text-slate-900">15 Haziran 2026</p>
                            </div>
                        </div>
                        <div className="flex items-center gap-3">
                            <CreditCard className="h-4 w-4 text-red-500" />
                            <div>
                                <p className="text-xs uppercase text-slate-400">Tutar</p>
                                <p className="text-lg font-bold text-red-600">4.500 TL</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Checkout;
