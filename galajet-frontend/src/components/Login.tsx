import { useState } from 'react';
import {
    ChevronDown,
    HelpCircle,
    Hotel,
    Lock,
    Mail,
    Plane,
    Car,
    Loader2,
} from 'lucide-react';
import { toast } from 'react-hot-toast';
import { supabaseClient } from '../services/supabaseClient';

const faqItems = [
    {
        question: 'Sifremi nasil sifirlarim?',
        answer:
            'E-posta adresinizi girip "Sifremi Unuttum" baglantisina tiklayin. Guvenli sifirlama e-postasi gonderecegiz.',
    },
    {
        question: 'Giris yaparken sorun yasiyorum, ne yapmaliyim?',
        answer:
            'E-posta ve sifrenizi kontrol edin. Hala sorun varsa destek ekibimiz sizinle aninda ilgilenir.',
    },
    {
        question: 'Giris yapmadan rezervasyonumu yonetebilir miyim?',
        answer:
            'Evet. "Rezervasyonlarim" alanindan PNR kodu ve soyadinizi girerek islemlerinizi yapabilirsiniz.',
    },
];

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [activeFaq, setActiveFaq] = useState<number | null>(0);

    const handleLogin = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!email || !password) {
            toast.error('Lutfen e-posta ve sifrenizi girin.');
            return;
        }

        try {
            setLoading(true);
            const { error } = await supabaseClient.auth.signInWithPassword({
                email,
                password,
            });

            if (error) {
                throw error;
            }

            toast.success('Welcome back, Captain!');
            window.location.assign('/dashboard');
        } catch (err) {
            const message = err instanceof Error ? err.message : 'Bir hata olustu.';
            toast.error(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 text-slate-900">
            <header className="border-b border-slate-900/10">
                <div className="bg-black text-slate-100">
                    <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-2 text-xs uppercase tracking-[0.2em]">
                        <div className="flex items-center gap-4">
                            <span className="flex items-center gap-2 text-red-500">
                                <HelpCircle className="h-4 w-4" />
                                Yardim
                            </span>
                            <span className="text-slate-400">|</span>
                            <span>FAQ</span>
                        </div>
                        <div className="flex items-center gap-4">
                            <button className="flex items-center gap-2 text-slate-300 transition hover:text-red-500">
                                Dil / Ulke
                            </button>
                            <button className="flex items-center gap-2 text-slate-300 transition hover:text-red-500">
                                Rezervasyonlarim
                            </button>
                        </div>
                    </div>
                </div>

                <div className="bg-slate-950 text-slate-100">
                    <div className="mx-auto flex max-w-6xl flex-col gap-4 px-6 py-4 md:flex-row md:items-center md:justify-between">
                        <div className="flex items-center gap-3">
                            <Plane className="h-8 w-8 text-red-600" />
                            <div>
                                <p className="text-lg font-semibold tracking-wide">GalaJet</p>
                                <p className="text-xs uppercase text-red-500">Premium Flight Booking</p>
                            </div>
                        </div>
                        <nav className="flex gap-6 text-sm font-semibold">
                            <button className="flex items-center gap-2 border-b-2 border-red-600 pb-1 text-red-500">
                                <Plane className="h-4 w-4" />
                                Ucuslar
                            </button>
                            <button className="flex items-center gap-2 border-b-2 border-transparent pb-1 text-slate-200 transition hover:border-red-600 hover:text-red-400">
                                <Hotel className="h-4 w-4" />
                                Oteller
                            </button>
                            <button className="flex items-center gap-2 border-b-2 border-transparent pb-1 text-slate-200 transition hover:border-red-600 hover:text-red-400">
                                <Car className="h-4 w-4" />
                                Arac Kiralama
                            </button>
                        </nav>
                    </div>
                </div>
            </header>

            <main>
                <section className="mx-auto grid max-w-6xl gap-10 px-6 py-12 lg:grid-cols-[1.2fr_0.8fr]">
                    <div className="relative overflow-hidden rounded-3xl">
                        <img
                            src="https://images.unsplash.com/photo-1469474968028-56623f02e42e?auto=format&fit=crop&w=1600&q=80"
                            alt="Ucak kalkisi"
                            className="h-full w-full object-cover"
                        />
                        <div className="absolute inset-0 bg-gradient-to-tr from-black/80 via-red-900/40 to-transparent" />
                        <div className="absolute bottom-6 left-6 right-6 rounded-2xl border border-white/10 bg-black/50 p-6 text-slate-100 backdrop-blur">
                            <p className="text-sm uppercase tracking-[0.4em] text-red-400">GalaJet</p>
                            <h2 className="mt-2 text-2xl font-semibold">Yolculugunuz burada baslar.</h2>
                            <p className="mt-2 text-sm text-slate-200">
                                Guvenli odeme, esnek iptal ve modern bir seyahat deneyimi.
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center">
                        <div className="w-full rounded-3xl border border-slate-200 bg-white p-8 shadow-xl">
                            <h1 className="text-2xl font-bold text-slate-950">GalaJet Hesabi: Giris Yap</h1>
                            <p className="mt-2 text-sm text-slate-500">
                                Seyahat planlarinizi tek bir yerden yonetin.
                            </p>

                            <form onSubmit={handleLogin} className="mt-8 space-y-6">
                                <div className="space-y-2">
                                    <label className="text-sm font-semibold text-slate-900">E-posta</label>
                                    <div className="flex items-center gap-3 rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus-within:border-red-500">
                                        <Mail className="h-5 w-5 text-red-500" />
                                        <input
                                            type="email"
                                            value={email}
                                            onChange={(event) => setEmail(event.target.value)}
                                            placeholder="ornek@galajet.com"
                                            className="w-full bg-transparent text-sm text-slate-900 outline-none"
                                            autoComplete="email"
                                        />
                                    </div>
                                </div>

                                <div className="space-y-2">
                                    <div className="flex items-center justify-between">
                                        <label className="text-sm font-semibold text-slate-900">Sifre</label>
                                        <button
                                            type="button"
                                            className="text-xs font-semibold text-red-600 hover:text-red-700"
                                        >
                                            Sifremi Unuttum
                                        </button>
                                    </div>
                                    <div className="flex items-center gap-3 rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus-within:border-red-500">
                                        <Lock className="h-5 w-5 text-red-500" />
                                        <input
                                            type="password"
                                            value={password}
                                            onChange={(event) => setPassword(event.target.value)}
                                            placeholder="********"
                                            className="w-full bg-transparent text-sm text-slate-900 outline-none"
                                            autoComplete="current-password"
                                        />
                                    </div>
                                </div>

                                <button
                                    type="submit"
                                    disabled={loading}
                                    className="flex w-full items-center justify-center gap-2 rounded-xl bg-red-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-70"
                                >
                                    {loading ? <Loader2 className="h-5 w-5 animate-spin" /> : null}
                                    Giris Yap
                                </button>

                                <p className="text-center text-sm text-slate-600">
                                    Hesabin yok mu?{' '}
                                    <span className="font-semibold text-red-600">Kayit Ol</span>
                                </p>
                            </form>
                        </div>
                    </div>
                </section>

                <section className="bg-white">
                    <div className="mx-auto max-w-6xl px-6 py-12">
                        <h2 className="text-2xl font-bold text-slate-950">Sik Sorulan Sorular</h2>
                        <p className="mt-2 text-sm text-slate-500">
                            Kisa cevaplarla sorularinizi hizla cozelim.
                        </p>

                        <div className="mt-6 divide-y divide-slate-200 rounded-2xl border border-slate-200">
                            {faqItems.map((item, index) => {
                                const isOpen = activeFaq === index;
                                return (
                                    <button
                                        type="button"
                                        key={item.question}
                                        onClick={() => setActiveFaq(isOpen ? null : index)}
                                        className="w-full px-6 py-5 text-left"
                                    >
                                        <div className="flex items-center justify-between">
                                            <span className="text-sm font-semibold text-slate-900">
                                                {item.question}
                                            </span>
                                            <ChevronDown
                                                className={`h-5 w-5 text-red-600 transition ${isOpen ? 'rotate-180' : ''
                                                    }`}
                                            />
                                        </div>
                                        {isOpen ? (
                                            <p className="mt-3 text-sm text-slate-600">{item.answer}</p>
                                        ) : null}
                                    </button>
                                );
                            })}
                        </div>
                    </div>
                </section>
            </main>

            <footer className="border-t border-slate-200 bg-slate-100">
                <div className="mx-auto flex max-w-6xl flex-col gap-4 px-6 py-6 text-xs text-slate-500 md:flex-row md:items-center md:justify-between">
                    <p>2026 GalaJet. Tum haklari saklidir.</p>
                    <div className="flex gap-6">
                        <button className="transition hover:text-red-600">Hakkimizda</button>
                        <button className="transition hover:text-red-600">Gizlilik</button>
                        <button className="transition hover:text-red-600">Kullanim Kosullari</button>
                    </div>
                </div>
            </footer>
        </div>
    );
}

export default Login;
