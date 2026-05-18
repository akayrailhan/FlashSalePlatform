import React, { createContext, useEffect, useState } from 'react';
import { supabaseClient } from '../services/supabaseClient';

type AuthContextValue = {
    user: any | null;
    token: string | null;
    isLoading: boolean;
    signOut: () => Promise<void>;
};

export const AuthContext = createContext<AuthContextValue>({
    user: null,
    token: null,
    isLoading: true,
    signOut: async () => { },
});

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<any | null>(null);
    const [token, setToken] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        let mounted = true;

        const init = async () => {
            try {
                const { data } = await supabaseClient.auth.getSession();
                if (!mounted) return;
                setUser(data.session?.user ?? null);
                setToken(data.session?.access_token ?? null);
            } finally {
                if (!mounted) return;
                setIsLoading(false);
            }
        };

        init();

        const { data: sub } = supabaseClient.auth.onAuthStateChange((_event, session) => {
            setUser(session?.user ?? null);
            setToken(session?.access_token ?? null);
        });

        return () => {
            mounted = false;
            sub.subscription.unsubscribe();
        };
    }, []);

    const signOut = async () => {
        await supabaseClient.auth.signOut();
        setUser(null);
        setToken(null);
    };

    return (
        <AuthContext.Provider value={{ user, token, isLoading, signOut }}>
            {children}
        </AuthContext.Provider>
    );
};

export default AuthProvider;
