import { useEffect, useState, useContext } from 'react';
import type { ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

type Props = {
    children: ReactNode;
};

export default function ProtectedRoute({ children }: Props) {
    const navigate = useNavigate();
    const [checking, setChecking] = useState(true);

    const { isLoading, token } = useContext(AuthContext);

    useEffect(() => {
        if (!isLoading && !token) {
            navigate('/login');
        } else if (!isLoading) {
            setChecking(false);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isLoading, token]);

    if (checking) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-black text-white">
                <div className="text-sm font-semibold">Oturum kontrol ediliyor...</div>
            </div>
        );
    }

    return <>{children}</>;
}
