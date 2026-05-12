import apiClient from './apiClient';

import type { IFlight } from '../types/flight';

export const getFlights = async (): Promise<IFlight[]> => {
    try {
        const response = await apiClient.get<IFlight[]>('/api/flights');
        return response.data;
    } catch (err) {
        const message = err instanceof Error ? err.message : 'Flight request failed.';
        throw new Error(message);
    }
};
